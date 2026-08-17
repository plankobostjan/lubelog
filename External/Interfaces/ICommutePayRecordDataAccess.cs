using CarCareTracker.Models;

namespace CarCareTracker.External.Interfaces
{
    public interface ICommutePayRecordDataAccess
    {
        public List<CommutePayRecord> GetCommutePayRecordsByVehicleId(int vehicleId);
        public CommutePayRecord GetCommutePayRecordById(int commutePayRecordId);
        public bool DeleteCommutePayRecordById(int commutePayRecordId);
        public bool SaveCommutePayRecordToVehicle(CommutePayRecord commutePayRecord);
        public bool DeleteAllCommutePayRecordsByVehicleId(int vehicleId);
    }
}
