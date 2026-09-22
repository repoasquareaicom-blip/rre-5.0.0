using System;
using System.Data;
using System.Data.SqlClient;

namespace Inventory.Masters
{
    public class LocationRackRepository
    {
        private readonly string connectionString = Program.connection;

        public DataTable GetLocations()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_Location_List", con))
            using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                ad.Fill(dt);
                return dt;
            }
        }

        public int InsertLocation(string locationName, int displayOrder, bool allowForSales, int updatedBy)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_Location_Insert", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@LocationName", locationName);
                cmd.Parameters.AddWithValue("@DisplayOrder", displayOrder);
                cmd.Parameters.AddWithValue("@AllowForSales", allowForSales);
                cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);
                con.Open();
                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        public int InsertLocation(string locationName, int displayOrder, int updatedBy)
        {
            return InsertLocation(locationName, displayOrder, true, updatedBy);
        }

        public void UpdateLocation(int locationId, string locationName, int displayOrder, bool allowForSales, int updatedBy)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_Location_Update", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@LocationId", locationId);
                cmd.Parameters.AddWithValue("@LocationName", locationName);
                cmd.Parameters.AddWithValue("@DisplayOrder", displayOrder);
                cmd.Parameters.AddWithValue("@AllowForSales", allowForSales);
                cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateLocation(int locationId, string locationName, int displayOrder, int updatedBy)
        {
            UpdateLocation(locationId, locationName, displayOrder, GetLocationAllowForSales(locationId), updatedBy);
        }

        private bool GetLocationAllowForSales(int locationId)
        {
            DataTable locations = GetLocations();
            foreach (DataRow row in locations.Rows)
            {
                if (row["LocationId"] != DBNull.Value && Convert.ToInt32(row["LocationId"]) == locationId)
                {
                    if (row.Table.Columns.Contains("AllowForSales") && row["AllowForSales"] != DBNull.Value)
                    {
                        bool allowForSales;
                        if (bool.TryParse(Convert.ToString(row["AllowForSales"]), out allowForSales))
                        {
                            return allowForSales;
                        }

                        int intValue;
                        return int.TryParse(Convert.ToString(row["AllowForSales"]), out intValue) && intValue != 0;
                    }

                    return true;
                }
            }

            return true;
        }

        public DataTable GetRacks(int locationId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_Rack_ListByLocation", con))
            using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@LocationId", locationId);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                return dt;
            }
        }

        public DataTable GetProductsByRack(int rackId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_Rack_ProductList", con))
            using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RackId", rackId);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                return dt;
            }
        }

        public int InsertRack(int locationId, string rackCaption, int updatedBy)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_Rack_Insert", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@LocationId", locationId);
                cmd.Parameters.AddWithValue("@RackCaption", rackCaption);
                cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);
                con.Open();
                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        public void UpdateRack(int rackId, string rackCaption, int updatedBy)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_Rack_Update", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RackId", rackId);
                cmd.Parameters.AddWithValue("@RackCaption", rackCaption);
                cmd.Parameters.AddWithValue("@UpdatedBy", updatedBy);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public int GetCurrentUserId()
        {
            int userId;
            if (int.TryParse(Program.userid, out userId))
            {
                return userId;
            }

            return 0;
        }
    }
}
