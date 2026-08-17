function showAddCommutePayRecordModal() {
    $.get('/Vehicle/GetAddCommutePayRecordPartialView', function (data) {
        if (data) {
            $("#commutePayRecordModalContent").html(data);
            initDatePicker($('#commutePayRecordDate'));
            initTagSelector($("#commutePayRecordTag"));
            $('#commutePayRecordModal').modal('show');
        }
    });
}
function showEditCommutePayRecordModal(commutePayRecordId, nocache) {
    if (!nocache) {
        var existingContent = $("#commutePayRecordModalContent").html();
        if (existingContent.trim() != '') {
            var existingId = getCommutePayRecordModelData().id;
            if (existingId == commutePayRecordId && $('[data-changed=true]').length > 0) {
                $('#commutePayRecordModal').modal('show');
                $('.cached-banner').show();
                return;
            }
        }
    }
    $.get(`/Vehicle/GetCommutePayRecordForEditById?commutePayRecordId=${commutePayRecordId}`, function (data) {
        if (data) {
            $("#commutePayRecordModalContent").html(data);
            initDatePicker($('#commutePayRecordDate'));
            initTagSelector($("#commutePayRecordTag"));
            $('#commutePayRecordModal').modal('show');
            bindModalInputChanges('commutePayRecordModal');
            $('#commutePayRecordModal').off('shown.bs.modal').on('shown.bs.modal', function () {
                if (getGlobalConfig().useMarkDown) {
                    toggleMarkDownOverlay("commutePayRecordNotes");
                }
            });
        }
    });
}
function hideAddCommutePayRecordModal() {
    $('#commutePayRecordModal').modal('hide');
}
function deleteCommutePayRecord(commutePayRecordId) {
    $("#workAroundInput").show();
    confirmDelete("Deleted Commute Pay Records cannot be restored.", (result) => {
        if (result.isConfirmed) {
            $.post(`/Vehicle/DeleteCommutePayRecordById?commutePayRecordId=${commutePayRecordId}`, function (data) {
                if (data.success) {
                    hideAddCommutePayRecordModal();
                    successToast("Commute Pay Record Deleted");
                    var vehicleId = GetVehicleId().vehicleId;
                    getVehicleCommutePayRecords(vehicleId);
                } else {
                    errorToast(data.message);
                    $("#workAroundInput").hide();
                }
            });
        } else {
            $("#workAroundInput").hide();
        }
    });
}
function saveCommutePayRecordToVehicle(isEdit) {
    var formValues = getAndValidateCommutePayRecordValues();
    if (formValues.hasError) {
        errorToast("Please check the form data");
        return;
    }
    $.post('/Vehicle/SaveCommutePayRecordToVehicleId', { commutePayRecord: formValues }, function (data) {
        if (data.success) {
            successToast(isEdit ? "Commute Pay Record Updated" : "Commute Pay Record Added.");
            hideAddCommutePayRecordModal();
            saveScrollPosition();
            getVehicleCommutePayRecords(formValues.vehicleId);
        } else {
            errorToast(data.message);
        }
    })
}
function getAndValidateCommutePayRecordValues() {
    var commutePayDate = $("#commutePayRecordDate").val();
    var commutePayDescription = $("#commutePayRecordDescription").val();
    var commutePayAmount = $("#commutePayRecordAmount").val();
    var commutePayNotes = $("#commutePayRecordNotes").val();
    var vehicleId = GetVehicleId().vehicleId;
    var commutePayRecordId = getCommutePayRecordModelData().id;
    var commutePayTags = $("#commutePayRecordTag").val();
    var hasError = false;
    var extraFields = getAndValidateExtraFields();
    if (extraFields.hasError) {
        hasError = true;
    }
    if (commutePayDate.trim() == '') {
        hasError = true;
        $("#commutePayRecordDate").addClass("is-invalid");
    } else {
        $("#commutePayRecordDate").removeClass("is-invalid");
    }
    if (commutePayDescription.trim() == '') {
        hasError = true;
        $("#commutePayRecordDescription").addClass("is-invalid");
    } else {
        $("#commutePayRecordDescription").removeClass("is-invalid");
    }
    if (commutePayAmount.trim() == '' || !isValidMoney(commutePayAmount)) {
        hasError = true;
        $("#commutePayRecordAmount").addClass("is-invalid");
    } else {
        $("#commutePayRecordAmount").removeClass("is-invalid");
    }
    return {
        id: commutePayRecordId,
        hasError: hasError,
        vehicleId: vehicleId,
        date: commutePayDate,
        description: commutePayDescription,
        amount: commutePayAmount,
        notes: commutePayNotes,
        tags: commutePayTags,
        files: uploadedFiles,
        extraFields: extraFields.extraFields
    }
}
