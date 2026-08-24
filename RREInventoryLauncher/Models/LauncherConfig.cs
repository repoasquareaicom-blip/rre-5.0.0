using System.Runtime.Serialization;

namespace RREInventoryLauncher.Models
{
    [DataContract]
    public class LauncherConfig
    {
        [DataMember]
        public string BranchCode { get; set; }

        [DataMember]
        public string ServerDeploymentPath { get; set; }

        [DataMember]
        public string ServerVersionFile { get; set; }

        [DataMember]
        public string LocalApplicationPath { get; set; }

        [DataMember]
        public string InventoryExe { get; set; }

        [DataMember]
        public string LogPath { get; set; }
    }
}
