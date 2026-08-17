using CarCareTracker.External.Interfaces;
using CarCareTracker.Models;
using CarCareTracker.Helper;
using LiteDB;

namespace CarCareTracker.External.Implementations
{
    public class CommutePayRecordDataAccess : ICommutePayRecordDataAccess
    {
        private ILiteDBHelper _liteDB { get; set; }
        private static string tableName = "commutepayrecords";
        public CommutePayRecordDataAccess(ILiteDBHelper liteDB)
        {
           _liteDB = liteDB;
        }
        public List<CommutePayRecord> GetCommutePayRecordsByVehicleId(int vehicleId)
        {
            var db = _liteDB.GetLiteDB();
            var table = db.GetCollection<CommutePayRecord>(tableName);
            var commutePayRecords = table.Find(Query.EQ(nameof(CommutePayRecord.VehicleId), vehicleId));
            return commutePayRecords.ToList() ?? new List<CommutePayRecord>();
        }
        public CommutePayRecord GetCommutePayRecordById(int commutePayRecordId)
        {
            var db = _liteDB.GetLiteDB();
            var table = db.GetCollection<CommutePayRecord>(tableName);
            return table.FindById(commutePayRecordId);
        }
        public bool DeleteCommutePayRecordById(int commutePayRecordId)
        {
            var db = _liteDB.GetLiteDB();
            var table = db.GetCollection<CommutePayRecord>(tableName);
            table.Delete(commutePayRecordId);
            db.Checkpoint();
            return true;
        }
        public bool SaveCommutePayRecordToVehicle(CommutePayRecord commutePayRecord)
        {
            var db = _liteDB.GetLiteDB();
            var table = db.GetCollection<CommutePayRecord>(tableName);
            table.Upsert(commutePayRecord);
            db.Checkpoint();
            return true;
        }
        public bool DeleteAllCommutePayRecordsByVehicleId(int vehicleId)
        {
            var db = _liteDB.GetLiteDB();
            var table = db.GetCollection<CommutePayRecord>(tableName);
            var commutePayRecords = table.DeleteMany(Query.EQ(nameof(CommutePayRecord.VehicleId), vehicleId));
            db.Checkpoint();
            return true;
        }
    }
}
