using CarCareTracker.Filter;
using CarCareTracker.Helper;
using CarCareTracker.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarCareTracker.Controllers
{
    public partial class VehicleController
    {
        [TypeFilter(typeof(CollaboratorFilter))]
        [HttpGet]
        public IActionResult GetCommutePayRecordsByVehicleId(int vehicleId)
        {
            var result = _commutePayRecordDataAccess.GetCommutePayRecordsByVehicleId(vehicleId);
            bool _useDescending = _config.GetUserConfig(User).UseDescending;
            if (_useDescending)
            {
                result = result.OrderByDescending(x => x.Date).ToList();
            }
            else
            {
                result = result.OrderBy(x => x.Date).ToList();
            }
            return PartialView("CommutePay/_CommutePayRecords", result);
        }
        [HttpPost]
        public IActionResult SaveCommutePayRecordToVehicleId(CommutePayRecordInput commutePayRecord)
        {
            if (!_userLogic.UserCanEditVehicle(GetUserID(), commutePayRecord.VehicleId, HouseholdPermission.Edit))
            {
                return Json(OperationResponse.Failed("Access Denied"));
            }
            commutePayRecord.Files = commutePayRecord.Files.Select(x => { return new UploadedFiles { Name = x.Name, Location = _fileHelper.MoveFileFromTemp(x.Location, "documents/") }; }).ToList();
            var result = _commutePayRecordDataAccess.SaveCommutePayRecordToVehicle(commutePayRecord.ToCommutePayRecord());
            if (result)
            {
                _eventLogic.PublishEvent(GetUserID(), WebHookPayload.FromCommutePayRecord(commutePayRecord.ToCommutePayRecord(), commutePayRecord.Id == default ? "commutepayrecord.add" : "commutepayrecord.update", User.Identity?.Name ?? string.Empty));
            }
            return Json(OperationResponse.Conditional(result, string.Empty, StaticHelper.GenericErrorMessage));
        }
        [HttpGet]
        public IActionResult GetAddCommutePayRecordPartialView()
        {
            return PartialView("CommutePay/_CommutePayRecordModal", new CommutePayRecordInput() { ExtraFields = _extraFieldDataAccess.GetExtraFieldsById((int)ImportMode.CommutePayRecord).ExtraFields });
        }
        [HttpGet]
        public IActionResult GetCommutePayRecordForEditById(int commutePayRecordId)
        {
            var result = _commutePayRecordDataAccess.GetCommutePayRecordById(commutePayRecordId);
            if (!_userLogic.UserCanEditVehicle(GetUserID(), result.VehicleId, HouseholdPermission.View))
            {
                return Redirect("/Error/Unauthorized");
            }
            var convertedResult = new CommutePayRecordInput
            {
                Id = result.Id,
                Amount = result.Amount,
                Date = result.Date.ToShortDateString(),
                Description = result.Description,
                Notes = result.Notes,
                VehicleId = result.VehicleId,
                Files = result.Files,
                Tags = result.Tags,
                ExtraFields = StaticHelper.AddExtraFields(result.ExtraFields, _extraFieldDataAccess.GetExtraFieldsById((int)ImportMode.CommutePayRecord).ExtraFields)
            };
            return PartialView("CommutePay/_CommutePayRecordModal", convertedResult);
        }
        private OperationResponse DeleteCommutePayRecordWithChecks(int commutePayRecordId)
        {
            var existingRecord = _commutePayRecordDataAccess.GetCommutePayRecordById(commutePayRecordId);
            if (!_userLogic.UserCanEditVehicle(GetUserID(), existingRecord.VehicleId, HouseholdPermission.Delete))
            {
                return OperationResponse.Failed("Access Denied");
            }
            var result = _commutePayRecordDataAccess.DeleteCommutePayRecordById(existingRecord.Id);
            if (result)
            {
                _eventLogic.PublishEvent(GetUserID(), WebHookPayload.FromCommutePayRecord(existingRecord, "commutepayrecord.delete", User.Identity?.Name ?? string.Empty));
            }
            return OperationResponse.Conditional(result, string.Empty, StaticHelper.GenericErrorMessage);
        }
        [HttpPost]
        public IActionResult DeleteCommutePayRecordById(int commutePayRecordId)
        {
            var result = DeleteCommutePayRecordWithChecks(commutePayRecordId);
            return Json(result);
        }
    }
}
