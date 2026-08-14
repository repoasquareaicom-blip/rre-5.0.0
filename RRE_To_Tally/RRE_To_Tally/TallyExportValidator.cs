namespace RRE_To_Tally;

public sealed class TallyExportValidator
{
    public ExportValidationResult ValidateInvoice(SalesExportInvoice invoice)
    {
        ExportValidationResult result = new ExportValidationResult();
        if (string.IsNullOrWhiteSpace(invoice.SalesId))
        {
            result.Errors.Add("Missing sales id.");
        }

        if (string.IsNullOrWhiteSpace(invoice.CustomerLedgerName))
        {
            result.Errors.Add("Missing customer name.");
        }

        if (invoice.Items.Count == 0)
        {
            result.Errors.Add("Invoice has no exportable items.");
        }

        foreach (SalesExportItem item in invoice.Items)
        {
            if (string.IsNullOrWhiteSpace(item.ProductTallyName))
            {
                result.Errors.Add("Missing product name for product id " + item.ProductId + ".");
            }

            if (item.Quantity == 0m)
            {
                result.Warnings.Add("Zero quantity for item " + item.ProductTallyName + ".");
            }
        }

        decimal grossDetailTotal = invoice.TaxableAmount + invoice.Cgst + invoice.Sgst + invoice.Igst;
        decimal expectedInvoiceTotal = Math.Round(grossDetailTotal - invoice.Discount + invoice.OtherCharges + invoice.RoundOff, 2, MidpointRounding.AwayFromZero);
        decimal balance = Math.Round(expectedInvoiceTotal - invoice.CalculatedTotal, 2, MidpointRounding.AwayFromZero);
        if (Math.Abs(balance) > 0.01m)
        {
            result.Errors.Add("Unbalanced voucher difference " + TallyNumericHelper.FormatAmount(balance) + ".");
        }

        if (result.Errors.Count > 0)
        {
            result.IsValid = false;
        }

        return result;
    }
}
