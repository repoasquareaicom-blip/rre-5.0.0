using System;
using System.Collections.Generic;

namespace Inventory.Report
{
    public class HsnSuggestion
    {
        public string HsnCode { get; set; }
        public string Category { get; set; }
        public string Confidence { get; set; }
        public string Reason { get; set; }
        public string Source { get; set; }
        public bool NeedsReview { get; set; }
    }

    public class HsnLookupRequest
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string CurrentHsn { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string Uom { get; set; }
        public string GstPercent { get; set; }
    }

    public class HsnSuggestionService
    {
        private readonly List<HsnSuggestionRule> rules = new List<HsnSuggestionRule>();
        private readonly OpenAiHsnLookupService openAiLookupService = new OpenAiHsnLookupService();

        public HsnSuggestionService()
        {
            LoadLocalRules();
        }

        public HsnSuggestion GetSuggestion(string productName)
        {
            HsnLookupRequest request = new HsnLookupRequest();
            request.ProductName = productName;
            return GetLocalSuggestion(request);
        }

        public HsnSuggestion GetLocalSuggestion(HsnLookupRequest request)
        {
            string normalizedName = Normalize(request == null ? string.Empty : request.ProductName);
            if (string.IsNullOrEmpty(normalizedName))
            {
                return NoMatch();
            }

            HsnSuggestion confirmedSuggestion = GetConfirmedMapping(normalizedName);
            if (confirmedSuggestion != null)
            {
                return confirmedSuggestion;
            }

            foreach (HsnSuggestionRule rule in rules)
            {
                if (rule.IsMatch(normalizedName))
                {
                    return new HsnSuggestion
                    {
                        HsnCode = rule.HsnCode,
                        Category = rule.Category,
                        Confidence = rule.Confidence,
                        Reason = rule.Reason,
                        Source = "Local Rule",
                        NeedsReview = string.IsNullOrEmpty(rule.HsnCode) || string.Equals(rule.Confidence, "Low", StringComparison.OrdinalIgnoreCase)
                    };
                }
            }

            return NoMatch();
        }

        public HsnSuggestion GetOnlineSuggestion(HsnLookupRequest request)
        {
            return openAiLookupService.Lookup(request);
        }

        public int OpenAiLookupCount
        {
            get { return openAiLookupService.RequestCount; }
        }

        public static bool IsValidHsn(string hsn)
        {
            string value = (hsn ?? string.Empty).Trim();
            if (value.Length != 4 && value.Length != 6 && value.Length != 8)
            {
                return false;
            }

            for (int i = 0; i < value.Length; i++)
            {
                if (!char.IsDigit(value[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private HsnSuggestion GetConfirmedMapping(string normalizedName)
        {
            // Extension point: confirmed product/category mappings can be checked here later.
            // Keep this local utility independent of any online HSN API.
            return null;
        }

        private void LoadLocalRules()
        {
            rules.Add(new HsnSuggestionRule(new string[] { "WIRE", "CABLE" }, "85444999", "Insulated electrical wire/cable", "High", "Product name matches electrical wire/cable keywords."));
            rules.Add(new HsnSuggestionRule(new string[] { "SWITCH", "SOCKET", "PLUG" }, "85365090", "Electrical switches, sockets and plugs", "High", "Product name matches switching/accessory keywords."));
            rules.Add(new HsnSuggestionRule(new string[] { "MCB", "RCCB", "MCCB", "ISOLATOR" }, "85362030", "Electrical protection equipment", "High", "Product name matches protection equipment keywords."));
            rules.Add(new HsnSuggestionRule(new string[] { "CONTACTOR", "RELAY" }, "85364900", "Electrical relays/contactors", "High", "Product name matches relay/contactor keywords."));
            rules.Add(new HsnSuggestionRule(new string[] { "PVC PIPE", "CONDUIT" }, "39172390", "PVC pipe/conduit", "High", "Product name matches PVC pipe/conduit keywords."));
            rules.Add(new HsnSuggestionRule(new string[] { "LED BULB", "LED LAMP", "LAMP" }, "85395000", "LED lamps", "Medium", "Product name matches lamp keywords; verify exact classification before saving."));
            rules.Add(new HsnSuggestionRule(new string[] { "FAN" }, "84145190", "Electric fan", "Medium", "Product name matches fan keyword; verify exact type before saving."));
            rules.Add(new HsnSuggestionRule(new string[] { "DB BOX", "DISTRIBUTION BOX" }, string.Empty, "Distribution box", "Low", "Product name matches DB box keywords, but local rules do not have a safe exact HSN."));
        }

        private static HsnSuggestion NoMatch()
        {
            return new HsnSuggestion
            {
                HsnCode = string.Empty,
                Category = string.Empty,
                Confidence = "No Match",
                Reason = "No reliable local rule matched.",
                Source = "Local",
                NeedsReview = true
            };
        }

        private static string Normalize(string value)
        {
            return (value ?? string.Empty).Trim().ToUpperInvariant();
        }

        private class HsnSuggestionRule
        {
            private readonly string[] keywords;

            public HsnSuggestionRule(string[] keywords, string hsnCode, string category, string confidence, string reason)
            {
                this.keywords = keywords;
                HsnCode = hsnCode;
                Category = category;
                Confidence = confidence;
                Reason = reason;
            }

            public string HsnCode { get; private set; }
            public string Category { get; private set; }
            public string Confidence { get; private set; }
            public string Reason { get; private set; }

            public bool IsMatch(string normalizedName)
            {
                for (int i = 0; i < keywords.Length; i++)
                {
                    if (normalizedName.IndexOf(keywords[i], StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
