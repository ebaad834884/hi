/// <reference path="jquery-2.1.4.js" />
var gduration;
var gES;
var gLS;
var gTaskNotes;
//Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
var gSRDesc;
var SSOIDinvalid = false;
var SSOID1Validate = false;
var fseskills1;
var fseskills;
var myselect;
var sendDate = "";
var receiveDate = "";
var responseTimeMs = 0;
var siebleTimeSpan = "";
var totalTimeSpan = "";
var clcikTimeSpan = "";
//SR Description changes Jayesh Soni
//var srdesc;
$(document).ready(function () {

    
    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins 
    if ($("#validSR").val() == "true") 
    {

        var message = "SR description is more than 256 characters – Edit the message as appropriate in SDT Booking screen";
        $("#SRtext").html(message);
        jQuery('#SR').modal('show');
        $("#CompleteSRDesc").append($("#completeSRDesc").val());
        $("#CompleteSRDesc").hide();
        $("#SRmoredetails").click(function () {
            $("#CompleteSRDesc").toggle();
            $("#HideCompleteSRDescDiv").show();
            $("#ShowCompleteSRDescDiv").hide();
        });

       $("#SRlessdetails").click(function () {
            $("#CompleteSRDesc").toggle();
            $("#ShowCompleteSRDescDiv").show();
            $("#HideCompleteSRDescDiv").hide();
        });              
    }
    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends

    if ($("#ValidateSystemId").val() == "SystemIDNotExistInSiebel") {
        var message = "System ID does not exist in Siebel.";
        $("#siteidtext").html(message);
        jQuery('#sitemodel').modal('show');
        $("#btnrequestappointment").attr("disabled", "disabled");
        $("#btnrequestappointment").removeClass("btn btn-info");
        $("#btnrequestappointment").addClass("btn btn-dark");
        $("#creVisNA").attr("disabled", "disabled");
        $("#creVisNA").removeClass("btn btn-info");
        $("#creVisNA").addClass("btn btn-dark");
        $("#btnPotentialDependencySite").attr("disabled", "disabled");
        $("#btnActualDependencySite").attr("disabled", "disabled");
        $("#btnPotentialDependencySystem").attr("disabled", "disabled");
        $("#btnActualDependencySystem").attr("disabled", "disabled");
        $("#btnSite").attr("disabled", "disabled");
        $("#btnSite").removeClass("btn btn-info");
        $("#btnSite").addClass("btn btn-dark");
        $("#btnSystem").attr("disabled", "disabled");
        $("#btnSystem").removeClass("btn btn-info");
        $("#btnSystem").addClass("btn btn-dark");
    }
    if ($("#ValidateSystemId").val() == "SystemIDNotExistInClick") {
        var message = "System ID does not exist in Click.";
        $("#siteidtext").html(message);
        jQuery('#sitemodel').modal('show');
        $("#btnrequestappointment").attr("disabled", "disabled");
        $("#btnrequestappointment").removeClass("btn btn-info");
        $("#btnrequestappointment").addClass("btn btn-dark");
        $("#creVisNA").attr("disabled", "disabled");
        $("#creVisNA").removeClass("btn btn-info");
        $("#creVisNA").addClass("btn btn-dark");
        $("#btnPotentialDependencySite").attr("disabled", "disabled");
        $("#btnActualDependencySite").attr("disabled", "disabled");
        $("#btnPotentialDependencySystem").attr("disabled", "disabled");
        $("#btnActualDependencySystem").attr("disabled", "disabled");
        $("#btnSite").attr("disabled", "disabled");
        $("#btnSite").removeClass("btn btn-info");
        $("#btnSite").addClass("btn btn-dark");
        $("#btnSystem").attr("disabled", "disabled");
        $("#btnSystem").removeClass("btn btn-info");
        $("#btnSystem").addClass("btn btn-dark");
    }

    if ($("#ValidateShipToSite").val() == "SiteIDNotExistInSiebel") {
        var message = "Site ID does not exist in Siebel.";
        $("#siteidtext").html(message);
        jQuery('#sitemodel').modal('show');      
    }

    if ($("#ValidateShipToSite").val() == "SiteIDNotExistInClick") {
        var message = "Site ID does not exist in Click.";
        $("#siteidtext").html(message);
        jQuery('#sitemodel').modal('show');      
    }

    if (($("#ValidateSystemId").val() == "SystemIDNotExistInClick") && ($("#ValidateShipToSite").val() == "SiteIDNotExistInClick")) {
        var message = "System ID/Site ID does not exist in Click.";
        $("#siteidtext").html(message);
        jQuery('#sitemodel').modal('show');
        $("#btnrequestappointment").attr("disabled", "disabled");
        $("#btnrequestappointment").removeClass("btn btn-info");
        $("#btnrequestappointment").addClass("btn btn-dark");
        $("#creVisNA").attr("disabled", "disabled");
        $("#creVisNA").removeClass("btn btn-info");
        $("#creVisNA").addClass("btn btn-dark");
        $("#btnPotentialDependencySite").attr("disabled", "disabled");
        $("#btnActualDependencySite").attr("disabled", "disabled");
        $("#btnPotentialDependencySystem").attr("disabled", "disabled");
        $("#btnActualDependencySystem").attr("disabled", "disabled");
        $("#btnSite").attr("disabled", "disabled");
        $("#btnSite").removeClass("btn btn-info");
        $("#btnSite").addClass("btn btn-dark");
        $("#btnSystem").attr("disabled", "disabled");
        $("#btnSystem").removeClass("btn btn-info");
        $("#btnSystem").addClass("btn btn-dark");
    }


    //Code Start - Tejashree - 13/04/2017 - Scope: US186 SystemID is NULL

    sessionStorage.setItem("systemIdAvailabilityChk", $("#ValidateSystemId").val());

    //Code End - Tejashree - 13/04/2017 - Scope: US186 SystemID is NULL

    $("#txtDelivery").val('');

    siebleTimeSpan = "Siebel Response(Sec): " + $("#hsiebleReceiveTime").val();
    $("#srepTime").html(siebleTimeSpan);

    if ($("#hdnappwindow").val() != "") {
        var $radios = $('input:radio[name=optionsRadios]');
        if ($("#hdnappwindow").val() == "TWO HOURS") {
            $radios.filter('[value="2 Hour"]').prop('checked', true);
        }
        if ($("#hdnappwindow").val() == "ONE HOUR") {
            $radios.filter('[value="1 Hour"]').prop('checked', true);
        }
        if ($("#hdnappwindow").val() == "AM-PM") {
            $radios.filter('[value="AM/PM"]').prop('checked', true);
        }
    }
    if ($("#hLstartEmptyvalue").val() == "1/1/0001 12:00:00 AM") {
        $("#txtlstart").val('');
    }
    if ($("#hEstartEmptyvalue").val() == "1/1/0001 12:00:00 AM") {
        $("#txtestart").val('');
    }
    $('#Skillfse2').attr("disabled", "disabled");
    $('#Skillfse3').attr("disabled", "disabled");
    $('#SSoid2').attr("disabled", "disabled");
    $('#SSoid3').attr("disabled", "disabled");

    $('#chkfse2').attr("disabled", "disabled");
    $('#chkfse3').attr("disabled", "disabled");

    $("#taskduration").val($('#tDuration').val());
    //Added to check whether it's Installlation job Type or not.
    if ($("#hsrType").val() != "Installation") {
        $('#lEStart').text($('#lES').val());
        $('#lLStart').text($('#lLS').val());
        gES = $("#txtestart").val();
        gLS = $("#txtlstart").val();

    }
    if ($("#hsrType").val() == "Installation") {
        gES = $("#txtestart").val();
        var totalvalue = $('#tInstallDuration').val() / 60;
        var totalHours = totalvalue % 8;
        var totaldays = parseInt(totalvalue / 8);
        var taskhrs = totaldays * 8;

        if (totalHours != 0)
            var taskhours = totalHours + " Hours";

        var secondHrs = taskhrs + " Hours";
        if (totaldays != 0)
            var taskdurationDays = totaldays + "days" + "(" + secondHrs + ")";
        $("#taskduration").val(taskhrs);
        $("#taskhrs").val(totalHours);
        var total = (parseInt(taskhrs)) + (parseInt(totalHours));
        $("#htDurationInstall").val(total);
        var htmlhrs = "= " + total + " Hour(s)";
        $("#totalhrs").html(htmlhrs);

    }
    if ($("#taskduration").val() == "" || $("#taskduration").val() == null) {
        gduration = "0";
    }
    else {
        gduration = $("#taskduration").val();
    }

    gTaskNotes = $("#tasknotes").val();
    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
    gSRDesc = $("#srdesc").val();
    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
    //SR Description changes Jayesh SOni
    //srdesc = sessionStorage.getItem("Desc");
    fseskills = $('#fseid1').val();
    var fse1data = $('#fseid1').val();
    var fse2data = $('#fseid2').val();
    var fse3data = $('#fseid3').val();
    $.ajax({
        type: "GET",
        url: "../XML/FSE.xml",
        dataType: "xml",
        success: function (xml) {

            var select = $('#Skillfse1');
            var select1 = $('#Skillfse2');
            var select2 = $('#Skillfse3');
            $(xml).find('FSE').each(function () {
                $(this).find('FseData').each(function () {

                    var OptionText = $(this).find('optionText').text();
                    var OptionValue = $(this).find('optionValue').text();

                    var option = $("<option>" + OptionText + "</option>");
                    option.attr("value", OptionValue);

                    select.append(option);

                });

            });


            if (fse1data != "") {
                $('#Skillfse1 option:contains("' + fse1data + '")').attr('selected', 'selected');

            }
            else {
                $('#Skillfse1').val(-1);
            }

            if (fse2data != "") {
                $('#Skillfse2 option:contains("' + fse2data + '")').attr('selected', 'selected');
            }
            else {
                $('#Skillfse2').val(-1);
            }

            if (fse3data != "") {
                $('#Skillfse3 option:contains("' + fse3data + '")').attr('selected', 'selected');
            }
            else {
                $('#Skillfse3').val(-1);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("some error");
        }
    });

    //Cancellation Job

    $.ajax({
        type: "GET",
        url: "../XML/FSE.xml",
        dataType: "xml",
        success: function (xml) {
            var CanclTaskDropdown = $('#CanclTaskDropdown');
            $(xml).find('CancelTaskValuesInSDT').each(function () {
                $(this).find('CancelTaskValues').each(function () {

                    var OptionText = $(this).find('optionText').text();
                    var OptionValue = $(this).find('optionValue').text();

                    var option = $("<option>" + OptionText + "</option>");
                    option.attr("value", OptionValue);

                    CanclTaskDropdown.append(option);
                });
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("some error");
        }
    });


    // Added fot Installation Job

    $('#SDTrequestAppointmentStartDte').val($('#happointmentStart').val());
    $('#SDTrequestAppointmentFinishDte').val($('#happointmentFinish').val());
    var SDTrequestAppointmentStartDte = $('#SDTrequestAppointmentStartDte')[0].placeholder;
    var SDTrequestAppointmentFinishDte = $('#SDTrequestAppointmentFinishDte')[0].placeholder;
    if (SDTrequestAppointmentStartDte == "1/1/0001")
        $('#APPSTART').hide();
    if (SDTrequestAppointmentFinishDte == "1/1/0001")
        $('#APPFINISH').hide();
    if ($('#hCREntitlementFlag').val() == "Y" && $('#hCRcontractFlag').val() == "Yes") {
        $('#txtlstart').val('');
        $('#txtestart').val('');
    }


    $('#checktaskid').val($('#hchecktaskid').val());
    $('#callIDdata').val($('#hcalltestiddata').val());
    $('#checkStatval').val($('#hcheckStatusVal').val());
    $('#checkfse').val($('#hcheckfse').val());

    var x = $('#checkStatval').val();
    if ($('#checkStatval').val() == "Cancelled") {

        $("#btnrequestappointment").attr("disabled", "disabled");
        $("#btnrequestappointment").removeClass("btn btn-info");
        $("#btnrequestappointment").addClass("btn btn-dark");
        $("#modVis").attr("disabled", "disabled");
        $("#modVis").removeClass("btn btn-warning");
        $("#modVis").addClass("btn btn-dark");


        $("#imgplus").removeAttr("onclick");
        $("#imgdel").removeAttr("onclick");

        $("#btnSite").attr("disabled", "disabled");
        $("#btnSystem").attr("disabled", "disabled");
        $("#btnCancelTask").attr("disabled", "disabled");
        $("#btnCancelTask").removeClass("btn btn-danger");
        $("#btnCancelTask").addClass("btn btn-dark");
    }

    if ($('#checkStatval').val() == "Assigned" || $('#checkStatval').val() == "Acknowledged" || $('#checkStatval').val() == "En Route" || $('#checkStatval').val() == "On Site" || $('#checkStatval').val() == "Completed" || $('#checkStatval').val() == "Incomplete" || $('#checkStatval').val() == "Cancelled" || $('#checkStatval').val() == "Rejected") {
        $("#btnrequestappointment").attr("disabled", "disabled");
        $("#btnrequestappointment").removeClass("btn btn-info");
        $("#btnrequestappointment").addClass("btn btn-dark");
        $("#modVis").attr("disabled", "disabled");
        $("#modVis").removeClass("btn btn-warning");
        $("#modVis").addClass("btn btn-dark");

        //PotentialDependency and ActualDependency for site and sytem should be in disable mode based on status.
        $("#btnPotentialDependencySite").attr("disabled", "disabled");
        $("#btnActualDependencySite").attr("disabled", "disabled");
        $("#btnPotentialDependencySystem").attr("disabled", "disabled");
        $("#btnActualDependencySystem").attr("disabled", "disabled");

        //Delete and Add button are disable when modify button is disable. Added by Phani kanth P.
        $("#imgplus").attr("disabled", "disabled");
        $("#imgdel").attr("disabled", "disabled");
    }

    //Commented for Instalaltion Job


    $(":input").inputmask();
    var html = "";
    var htm = "";
    $('#birthday').daterangepicker({
        singleDatePicker: true,
        calender_style: "picker_4"
    }, function (start, end, label) {
        console.log(start.toISOString(), end.toISOString(), label);
    });
    $('.editOption').hover(function () {
        $('.editOption').show();
    });



    $('.selectpicker').on('change', function () {

        if ($("#hsrType").val() == "Installation") {
            if ($('.selectpicker').val() != null) {
                if ($('.selectpicker').val().indexOf("EnterSSOID") == "-1") {
                    myselect = $(".selectpicker").val();
                    SSOID1Validate = true;

                    $('.editOption')[0].value = ""
                    $('.editOption').hide();
                    $("#imgPreferedPSEValid").hide();
                    $("#imgPreferedPSECross").hide();
                }
                else {
                    $('.editOption').show();
                }

            } else {
                $('.editOption')[0].value = ""
                $('.editOption').hide();
                $("#imgPreferedPSEValid").hide();
                $("#imgPreferedPSECross").hide();
            }
        }
        else {
            myselect = $(".selectpicker").val();
            SSOID1Validate = true;
        }

    });

    var htm = "";
    $.ajax({
        type: "POST",
        url: "../Home/GetFSE1ddlItems",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            for (var i = 0; i < data.length; i++) {

                htm += '<option value=' + data[i].Value + '>' + data[i].Value + ' || ' + data[i].Text + ' || SDT Schedule</option>';
            }

            if ($("#hsrType").val() == "Installation") {

                htm += '<option value=' + 'EnterSSOID' + '>' + 'Enter SSO ID' + '</option>';
            }
            $(".selectpicker").append(htm);


            for (var i = 0; i < data.length; i++) {
                if (data[i].Selected) {
                    $(".selectpicker option[value=" + data[i].Value + "]").attr('selected', 'selected');
                    $("#imgPreferedPSE1").show();
                }

            }

            if ($("#hsrType").val() == "Installation") {
                var str = $("#hdnReqPrefFlag").val();
                if ($("#hdnReqPrefFlag").val() == "R") {
                    $("#chkfse1").prop("checked", true);
                    $(".icheckbox_flat-green").addClass("checked");
                } else {
                    $("#chkfse1").prop("checked", false);
                    $(".icheckbox_flat-green").removeClass("checked");
                }
            }
            else {
                var str = $("#hdnReqPrefFlag").val();
                if ($("#hdnReqPrefFlag").val() == "R") {
                    $("#chkfse1").prop("checked", true);
                    $(".icheckbox_flat-green").addClass("checked");

                } else {
                    $("#chkfse1").prop("checked", false);
                    $(".icheckbox_flat-green").removeClass("checked");
                }
            }


            $(".selectpicker option").each(function () {
                $(this).siblings("[value=" + this.value + "]").remove();
            });
            $(".selectpicker").selectpicker('refresh');

        }
    })



    var sad = document.getElementById("checkStatval").value;
    if (sad == "Tentative") {
        $("#hometab").css("background-color", "#4FB5D3");
    }
    else {
        $("#hometab").css("background-color", "none");
    }

});

$(function () {
    $('#taskduration').keypress(function (evt) {
        var charCode = (evt.which) ? evt.which : evt.keyCode;
        if ((charCode > 31 && (charCode < 48 || charCode > 57)) || charCode == 45) {
            return false;
        }
        return true;
    });

    $("#btnrequestappointment").click(function (e) {

        e.preventDefault();
        var SSOID1;
        var SSOID2;
        var SSOID3;
        var ID1;
        var ID2;
        var ID3;
        var fseSkill;
        var isrequiredfse;
        isrequiredfse = $("#chkfse1").is(":checked");

        //Skill 2 & Skill 3 removed by Phani Kanth P.
        fseSkill = $("#Skillfse1").val();
        var values = '';
        if ($("#hsrType").val() == "Installation") {
            if ($('.selectpicker').val() != null) {
                if ($('.selectpicker').val().indexOf("EnterSSOID") == "-1") {
                    if ($('.editOption').val() == '') {
                        values = $(".selectpicker").val()
                    }
                    else {
                        if ($(".selectpicker").val() == null) {
                            values = $('.editOption').val();
                        } else {
                            values = $(".selectpicker").val() + $('.editOption').val();
                        }
                    }
                } else {

                    if ($('.editOption').val() == '') {
                        values = $(".selectpicker").val()
                        values = values.slice(0, -1);
                    }
                    else {
                        if ($(".selectpicker").val() == null) {
                            values = $('.editOption').val();
                        } else {
                            values = $(".selectpicker").val();
                            values = values.slice(0, -1) + ',' + $('.editOption').val();
                            if (values.substring(0, 1) == ",") {
                                values = values.split(',').slice(1);
                            }

                        }
                    }


                }
            }
        }
        else {
            values = $(".selectpicker").val();
        }
        if (values != null) {
            SSOID1 = values.toString().split('||');
            ID1 = SSOID1[0]
        }
        else {
            ID1 = null;
        }
        var select = $("#SSoid3").val();
        if (select != null) {
            SSOID3 = select.toString().split('||');
            ID3 = SSOID3[0]
        }
        else {
            ID3 = null;
        }
        var val = $("#SSoid2").val();
        if (val != null) {
            SSOID2 = val.toString().split('||');
            ID2 = SSOID2[0]
        }
        else {
            ID2 = null;
        }
        var isDisabled = $('#txtDelivery').prop('disabled');

        if ($('#hsrType').val() == "Installation") {
            var IsInstallValidationsFail = taskDatetimeValidations()
            if (IsInstallValidationsFail)
                return true;
        }
        else {
            var AppWindow = $('#appWindow input:radio:checked');

            if ($('#hsrType').val() == "Corrective Repair" && $('#hCREntitlementFlag').val() == "Y" && $("#txtestart").val() == "" || $("#txtlstart").val() == "") {


            }
            else {
                var IsValidationFail = EarlyStartAndLateStartValidations();
                if (IsValidationFail)
                    return true;
            }
        }
        if ($('#hsrType').val() == "Corrective Repair" && $('#hCREntitlementFlag').val() == "Y" && $("#txtestart").val() == "") {

        }
        else {
            if (!isDisabled) {
                var IsDeliveryDateValidationFail = DeliveryDateValidations()

                if (IsDeliveryDateValidationFail)
                    return true;
            }
        }

        if ($('#hsrType').val() == "Installation") {
            if ($('.editOption').val() != "") {
                if (SSOIDinvalid == false) {
                    jQuery('#modal-2').modal('show')
                    $("#text").html("SSOID is Invalid. Please enter a valid SSOID");
                    return;
                }
            }

            if ($("#htDurationInstall").val() == 0 || $("#htDurationInstall").val() == null) {
                $("#text").html("Task Duration can't be zero. Please enter...");
                jQuery('#modal-2').modal('show')
                $('#taskduration').focus();
                return true;
            }

        }



        if ($('#hsrType').val() != "Installation") {
            if ($("#taskduration").val() == 0 || $("#taskduration").val() == null) {
                jQuery('#modal-2').modal('show')

                $("#text").html("Visit Duration can't be zero. Please enter...")
                $('#taskduration').focus();
                return true;
            }

        }


        //prajna  Change for US85 starts (Same ES and LS) New code added  for Change asked

        //if ($('#hsrType').val() == "Corrective Repair" && $('#hCREntitlementFlag').val() == "Y" && $("#txtestart").val() == "" && $("#txtlstart").val() == "" && $("#txtestart").val() == $("#txtlstart").val()) {
        //    jQuery('#modal-2').modal('show')

        //    $("#text").html("Contracts are not loaded in click. Please refer to contracts in Siebel CRM and enter the ES and LS values manually.")

        //    return true;
        //}

        //prajna  Change for US85 ends (Same ES and LS) New code added  for Change asked

        //Code Start-Prajna - 14/04/2017 - US178/TA1305
        if ($('#hsrType').val() == "Corrective Repair" && $('#hCREntitlementFlag').val() == "Y" && $('#hclickContractAvailabilityFlag').val() == "N")
        {
            // Prajna Splited one if condition in to two, To check if one of the values in ES,LS is blank inside above if condtion for US178/TA1305
            if ($("#txtestart").val() == "" && $("#txtlstart").val() == "")
            {
                jQuery('#modal-2').modal('show')
                $("#text").html("Contracts are not loaded in click. Please refer to contracts in Siebel CRM and enter the ES and LS values manually.")
                return true;
            }
            //Code Start-Prajna - 14/04/2017 - US178/TA1305
            //Condition to check if ES is blank and LS has some values in the Home page and give popup
            if ($("#txtestart").val() == "" && $("#txtlstart").val() != "") {
                jQuery('#modal-2').modal('show')

                $("#text").html("Please enter ES to proceed ahead")
                return true;

            }
            ////Condition to check if LS is blank and ES has some values in the Home page and give popup
            if ($("#txtestart").val() != "" && $("#txtlstart").val() == "") {
                jQuery('#modal-2').modal('show')
                $("#text").html("Please enter LS to proceed ahead")
                return true;

            }
            //Code End-Prajna - 14/04/2017 - US178/TA1305
        }

        
        var deliverytype = $('#deliverytype :selected').text();

        var partComment = $("#txtPartComments").val();

        var deliveryDate = $("#txtDelivery").val();
        var address = $("#address").val();
        if (deliverytype != "" && deliverytype != "--Please Select--") {
            AddOrUpdateComment2(deliverytype, partComment, deliveryDate, address);
        }
        if (deliverytype != "" && deliverytype != "--Please Select--") {
            AddOrUpdateComment(deliverytype, partComment, deliveryDate, address);
        }
        var s = '';

        //



        //Modify Visit purpose
        if ($("#hTaskExistsStatus").val() == "Exists") {
            for (i = 0; i < addr2.length; i++) {

                for (j = 0; j < addr2[i].length; j++) {
                    //No need to chnage change the Name
                    if (addr2[i][j][4] == "South Korea") {

                        addr2[i][j][4] = "KOREA, REPUBLIC OF";
                    }
                    //Before Modify Check whether Customer site selected or not
                    if (j == 0)
                        if (addr2[i].length == 1) {

                            s += '"' + addr2[i][j] + '"';
                        }
                        else
                            s += '"' + addr2[i][j] + '"' + '=';
                    else
                        if (j != (addr2[i].length - 1)) {
                            s += '"' + addr2[i][j] + '"' + '=';
                        }
                        else {
                            s += '"' + addr2[i][j] + '"';
                        }

                }
            }
        }
        else {
            //Create Visit purpose


            for (i = 0; i < addr1.length; i++) {

                for (j = 0; j < addr1[i].length; j++) {
                    if (addr1[i][j][4] == "South Korea") {

                        addr1[i][j][4] = "KOREA, REPUBLIC OF";
                    }
                    //Before Create Check whether Customer site selected or not
                    if (j == 0)
                        if (addr1[i].length == 1) {
                            s += '"' + addr1[i][j] + '"';
                        }
                        else
                            s += '"' + addr1[i][j] + '"' + '=';
                    else
                        if (j != (addr1[i].length - 1)) {
                            s += '"' + addr1[i][j] + '"' + '=';
                        }
                        else {
                            s += '"' + addr1[i][j] + '"';
                        }

                }
            }
        }


        s += ",";
        if (s == ",") {
            s = "";
        }
        var tasknotes = $('#tasknotes').val();
        if (tasknotes != "") {

            if (tasknotes.indexOf(',') != -1) {
                tasknotes = tasknotes.replace(/,/g, "");

            }
        }

        //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
        var srdesc = $('#srdesc').val();
        //if (srdesc != "")
        //{
        //    if (srdesc.indexof(',') != -1)
        //    {
        //        srdesc = srdesc.replace(/,/g, "");
        //    }
        //}
        //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends



        var parComments = $('#txtPartComments').val();

        if (parComments != "") {
            if (parComments.indexOf(',') != -1) {
                parComments = parComments.replace(/,/g, "");
            }
        }

        //Jayesh Soni - US82 - 13/04/2017 - Start
        //var SRDescription = $('#txtSRDescription').val();
        //Jayesh Soni - US82 - 13/04/2017 - End

        // Changes done by Raju, send array to controller intead of string
        if ($("#hsrType").val() == "Installation") {
            var dataToPost = {
                Duration: $("#htDurationInstall").val(),
                Profile: "AM/PM",
                TaskNotes: tasknotes,                              
                SSOFse1: ID1,
                SSOFse2: ID2,
                SSOFse3: ID3,
                fseSkill: fseSkill,
                taskStatus: $('#checkStatval').val(),
                addressArray: addr2,
                desiredDate: $("#txtestart").val(),
                partcomments: parComments,
                JobType: "Install",
                IsRequiredfse: isrequiredfse,
                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
                SRDesc: srdesc
                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
            };

        }
        else {
            var dataToPost = {
                EStart: $("#txtestart").val(),
                LStart: $("#txtlstart").val(),
                Duration: $("#taskduration").val().toString(),
                Profile: AppWindow.attr('value'),
                TaskNotes: tasknotes,
                SSOFse1: ID1,
                SSOFse2: ID2,
                SSOFse3: ID3,
                fseSkill: fseSkill,
                taskStatus: $('#checkStatval').val(),
                addressArray: addr2,
                desiredDate: $('#desireddate').val(),
                partcomments: parComments,
                JobType: "Other",
                IsRequiredfse: isrequiredfse,
                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
                SRDesc: srdesc
                //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
                
            };
        }

        $.ajax({
            url: '../RequestAppointmentBooking/RequestAppointment',
            type: "POST",
            data: dataToPost,
            success: function (data) {
                if (data.HasError) {
                    alert(data.Message);
                }
                else {
                    window.location.href = "../RequestAppointmentBooking/RequestAppointment/";
                }
            }
        });
        return true

    })

});



$("#taskduration").change(function () {
    var taskduration = $("#taskduration").val();
    var taskhrs = $("#taskhrs").val();
    if (taskhrs == null) {
        taskhrs = 0;
    }
    var total = (parseInt(taskduration)) + (parseInt(taskhrs));
    $("#htDurationInstall").val(total);
    var htmlhrs = "= " + total + " Hour(s)";
    $("#totalhrs").html(htmlhrs);

});
$("#taskhrs").change(function () {
    var taskduration = $("#taskduration").val();
    var taskhrs = $("#taskhrs").val();
    if (taskduration == null) {
        taskduration = 0;
    }
    var total = (parseInt(taskduration)) + (parseInt(taskhrs));
    $("#htDurationInstall").val(total);
    var htmlhrs = "= " + total + " Hour(s)";
    $("#totalhrs").html(htmlhrs);

});
function CanclDropdown() {
    if ($("#CanclTaskDropdown").val() == 0) {
        document.getElementById("cancltask").disabled = true;
    }
    else {
        document.getElementById("cancltask").disabled = false;
    }
}
function imagefse1() {
    var values = $(".selectpicker").val();

    $("#imgPreferedPSE1").show();
    if (values == null) {
        $("#imgPreferedPSE1").hide();
    }


}
function ImageFse2() {

    var values = $("#SSoid2").val();



    $("#imgPreferedPSE2").show();
    if (values == null) {
        $("#imgPreferedPSE2").hide();
    }

}
function ImageFse3() {
    var select = $("#SSoid3").val();

    $("#imgPreferedPSE3").show();
    if (select == null) {
        $("#imgPreferedPSE3").hide();
    }
}
$("#ClosedDiv").click(function () {
    window.close();
})

$('#btnPotentialDependencySite').click(function () {
    //added by barnali for part pick up 



    var SSOID1;
    var SSOID2;
    var SSOID3;
    var ID1;
    var ID2;
    var ID3;
    var fseSkill;
    var isrequiredfse;
    isrequiredfse = $("#chkfse1").is(":checked");
    //Comment by Phani Kanth because of skill 2 & skill 3 are disabled
    fseSkill = $("#Skillfse1").val();
    var values = '';
    if ($("#hsrType").val() == "Installation") {
        if ($('.selectpicker').val() != null) {
            if ($('.selectpicker').val().indexOf("EnterSSOID") == "-1") {
                if ($('.editOption').val() == '') {
                    values = $(".selectpicker").val()
                }
                else {
                    if ($(".selectpicker").val() == null) {
                        values = $('.editOption').val();
                    } else {
                        values = $(".selectpicker").val() + $('.editOption').val();
                    }
                }
            } else {

                if ($('.editOption').val() == '') {
                    values = $(".selectpicker").val()
                    values = values.slice(0, -1);
                }
                else {
                    if ($(".selectpicker").val() == null) {
                        values = $('.editOption').val();
                    } else {
                        values = $(".selectpicker").val();
                        values = values.slice(0, -1) + ',' + $('.editOption').val();
                        if (values.substring(0, 1) == ",") {
                            values = values.split(',').slice(1);
                        }

                    }
                }


            }
        }
    }
    else {
        values = $(".selectpicker").val();
    }
    if (values != null) {
        SSOID1 = values.toString().split('||');
        ID1 = SSOID1[0]
    }
    else {
        ID1 = null;
    }
    var select = $("#SSoid3").val();
    if (select != null) {
        SSOID3 = select.toString().split('||');
        ID3 = SSOID3[0]
    }
    else {
        ID3 = null;
    }
    var val = $("#SSoid2").val();
    if (val != null) {
        SSOID2 = val.toString().split('||');
        ID2 = SSOID2[0]
    }
    else {
        ID2 = null;
    }
    var isDisabled = $('#txtDelivery').prop('disabled');

    if ($('#hsrType').val() == "Installation") {
        var IsInstallValidationsFail = taskDatetimeValidations()
        if (IsInstallValidationsFail)
            return true;
    }
    else {
        var AppWindow = $('#appWindow input:radio:checked');

        if ($('#hsrType').val() == "Corrective Repair" && $('#hCREntitlementFlag').val() == "Y" && $("#txtestart").val() == "" || $("#txtlstart").val() == "") {


        }
        else {
            var IsValidationFail = EarlyStartAndLateStartValidations();
            if (IsValidationFail)
                return true;
        }
    }
    if ($('#hsrType').val() == "Corrective Repair" && $('#hCREntitlementFlag').val() == "Y" && $("#txtestart").val() == "") {

    }
    else {
        if (!isDisabled) {
            var IsDeliveryDateValidationFail = DeliveryDateValidations()

            if (IsDeliveryDateValidationFail)
                return true;
        }
    }


    if ($('#hsrType').val() == "Installation") {
        if ($('.editOption').val() != "") {
            if (SSOIDinvalid == false) {
                jQuery('#modal-2').modal('show')
                $("#text").html("SSOID is Invalid. Please enter a valid SSOID");
                return;
            }
        }

        if ($("#htDurationInstall").val() == 0 || $("#htDurationInstall").val() == null) {
            jQuery('#modal-2').modal('show')
            $("#text").html("Task Duration can't be zero. Please enter...");
            $('#taskduration').focus();
            return;
        }

    }

    if ($('#hsrType').val() != "Installation") {
        if ($("#taskduration").val() == 0 || $("#taskduration").val() == null) {
            jQuery('#modal-2').modal('show')

            $("#text").html("Visit Duration can't be zero. Please enter...")
            $('#taskduration').focus();
            return true;
        }

    }

    if (GetSiteSystemData.SiteCountPotential <= 0) {
        jQuery('#modal-2').modal('show')
        $("#text").html("No Potential Site Dependency Job Found.")
        return true;
    }

    var deliverytype = $('#deliverytype :selected').text();

    var partComment = $("#txtPartComments").val();

    var deliveryDate = $("#txtDelivery").val();
    var address = $("#address").val();
    if (deliverytype != "" && deliverytype != "--Please Select--") {
        AddOrUpdateComment2(deliverytype, partComment, deliveryDate, address);
    }
    if (deliverytype != "" && deliverytype != "--Please Select--") {
        AddOrUpdateComment(deliverytype, partComment, deliveryDate, address);
    }

    var s = '';


    for (i = 0; i < addr2.length; i++) {

        for (j = 0; j < addr2[i].length; j++) {
            //No need to chnage change the Name
            if (addr2[i][j][4] == "South Korea") {

                addr2[i][j][4] = "KOREA, REPUBLIC OF";
            }
            //Before Modify Check whether Customer site selected or not
            if (j == 0)
                if (addr2[i].length == 1) {

                    s += '"' + addr2[i][j] + '"';
                }
                else
                    s += '"' + addr2[i][j] + '"' + '=';
            else
                if (j != (addr2[i].length - 1)) {
                    s += '"' + addr2[i][j] + '"' + '=';
                }
                else {
                    s += '"' + addr2[i][j] + '"';
                }

        }
    }


    s += ",";
    if (s == ",") {
        s = "";
    }
    var tasknotes = $('#tasknotes').val();

    if (tasknotes != "") {

        if (tasknotes.indexOf(',') != -1) {
            tasknotes = tasknotes.replace(/,/g, "");
        }
    }

    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
    var srdesc = $('#srdesc').val();
    if (srdesc != "")
    {
        if (srdesc.indexOf(',') != -1)
        {
            srdesc = srdesc.replace(/,/g, "");
        }

    }
    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends

    // Changes done by Raju, send array to controller intead of string

    var installDuration = ($("#htDurationInstall").val() * 60).toString();
    if ($("#hsrType").val() == "Installation") {
        var dataToPost = {

            TaskSiteID: $('#hTaskSiteID').val(),
            SiteCountActual: GetSiteSystemData.SiteCountActual,
            EStart: $("#txtestart").val(),
            LStart: "",
            Duration: installDuration,
            TaskNotes: tasknotes,
            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
            SRDesc: srdesc,
            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
            DependencyType: "PotentialDependency",
            addressArray: addr2,
            Profile: "AM/PM",
            SSOFse1: ID1,
            fseSkill: fseSkill,
            IsRequiredfse: isrequiredfse,
            JobType: $("#hsrType").val()
        };
    }
    else {

        var dataToPost = {

            TaskSiteID: $('#hTaskSiteID').val(),
            SiteCountActual: GetSiteSystemData.SiteCountActual,
            EStart: $("#txtestart").val(),
            LStart: $("#txtlstart").val(),
            Duration: $("#taskduration").val().toString(),
            TaskNotes: tasknotes,
            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
            SRDesc: srdesc,
            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
            DependencyType: "PotentialDependency",
            addressArray: addr2,
            Profile: AppWindow.attr('value'),
            SSOFse1: ID1,
            fseSkill: fseSkill,
            IsRequiredfse: isrequiredfse,
            JobType: $("#hsrType").val()

        };

    }


    $.ajax({
        url: '../Site/SiteDependencies',
        type: "Post",
        data: dataToPost,
        success: function (data) {
            if (data.HasError) {
                alert(data.Message);
            }
            else {
                window.location.href = "../Site/SiteDependencies/";
            }
        }
    });
    return true
});

$('#btnActualDependencySite').click(function () {
    var SSOID1;
    var SSOID2;
    var SSOID3;
    var ID1;
    var ID2;
    var ID3;
    var fseSkill;
    var isrequiredfse;
    isrequiredfse = $("#chkfse1").is(":checked");
    fseSkill = $("#Skillfse1").val();
    var values = '';
    if ($("#hsrType").val() == "Installation") {
        if ($('.selectpicker').val() != null) {
            if ($('.selectpicker').val().indexOf("EnterSSOID") == "-1") {
                if ($('.editOption').val() == '') {
                    values = $(".selectpicker").val()
                }
                else {
                    if ($(".selectpicker").val() == null) {
                        values = $('.editOption').val();
                    } else {
                        values = $(".selectpicker").val() + $('.editOption').val();
                    }
                }
            } else {

                if ($('.editOption').val() == '') {
                    values = $(".selectpicker").val()
                    values = values.slice(0, -1);
                }
                else {
                    if ($(".selectpicker").val() == null) {
                        values = $('.editOption').val();
                    } else {
                        values = $(".selectpicker").val();
                        values = values.slice(0, -1) + ',' + $('.editOption').val();
                        if (values.substring(0, 1) == ",") {
                            values = values.split(',').slice(1);
                        }

                    }
                }


            }
        }
    }
    else {
        values = $(".selectpicker").val();
    }
    if (values != null) {
        SSOID1 = values.toString().split('||');
        ID1 = SSOID1[0]
    }
    else {
        ID1 = null;
    }
    var select = $("#SSoid3").val();
    if (select != null) {
        SSOID3 = select.toString().split('||');
        ID3 = SSOID3[0]
    }
    else {
        ID3 = null;
    }
    var val = $("#SSoid2").val();
    if (val != null) {
        SSOID2 = val.toString().split('||');
        ID2 = SSOID2[0]
    }
    else {
        ID2 = null;
    }
    var isDisabled = $('#txtDelivery').prop('disabled');

    if ($('#hsrType').val() == "Installation") {
    }
    else {
        var AppWindow = $('#appWindow input:radio:checked');

    }

    var deliverytype = $('#deliverytype :selected').text();

    var partComment = $("#txtPartComments").val();

    var deliveryDate = $("#txtDelivery").val();
    var address = $("#address").val();
    if (deliverytype != "" && deliverytype != "--Please Select--") {
        AddOrUpdateComment2(deliverytype, partComment, deliveryDate, address);
    }
    if (deliverytype != "" && deliverytype != "--Please Select--") {
        AddOrUpdateComment(deliverytype, partComment, deliveryDate, address);
    }

    var s = '';



    for (i = 0; i < addr2.length; i++) {

        for (j = 0; j < addr2[i].length; j++) {
            //No need to chnage change the Name
            if (addr2[i][j][4] == "South Korea") {

                addr2[i][j][4] = "KOREA, REPUBLIC OF";
            }
            //Before Modify Check whether Customer site selected or not
            if (j == 0)
                if (addr2[i].length == 1) {

                    s += '"' + addr2[i][j] + '"';
                }
                else
                    s += '"' + addr2[i][j] + '"' + '=';
            else
                if (j != (addr2[i].length - 1)) {
                    s += '"' + addr2[i][j] + '"' + '=';
                }
                else {
                    s += '"' + addr2[i][j] + '"';
                }

        }
    }

    s += ",";
    if (s == ",") {
        s = "";
    }
    var tasknotes = $('#tasknotes').val();
    if (tasknotes != "") {
        if (tasknotes.indexOf(',') != -1) {
            tasknotes = tasknotes.replace(/,/g, "");
        }
    }

    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
    var srdesc = $('#srdesc').val();
    if (srdesc != "") {
        if (srdesc.indexOf(',') != -1) {
            srdesc = srdesc.replace(/,/g, "");
        }
    }
    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends

    // Changes done by Raju, send array to controller intead of string

    var installDuration = ($("#htDurationInstall").val() * 60).toString();
    if ($("#hsrType").val() == "Installation") {
        var dataToPost = {

            TaskSiteID: $('#hTaskSiteID').val(),
            EStart: $("#txtestart").val(),
            LStart: "",
            Duration: installDuration,
            TaskNotes: tasknotes,
            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
            SRDesc: srdesc,
            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
            DependencyType: "ActualDependency",
            addressArray: addr2,
            Profile: "AM/PM",
            SSOFse1: ID1,
            fseSkill: fseSkill,
            IsRequiredfse: isrequiredfse,
            JobType: $("#hsrType").val()
        };
    }
    else {

        var dataToPost = {
            TaskSiteID: $('#hTaskSiteID').val(),
            EStart: $("#txtestart").val(),
            LStart: $("#txtlstart").val(),
            Duration: $("#taskduration").val().toString(),
            TaskNotes: tasknotes,
            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
            SRDesc: srdesc,
            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
            DependencyType: "ActualDependency",
            addressArray: addr2,
            Profile: AppWindow.attr('value'),
            SSOFse1: ID1,
            fseSkill: fseSkill,
            IsRequiredfse: isrequiredfse,
            JobType: $("#hsrType").val()

        };

    }


    $.ajax({
        url: '../Site/SiteDependencies',
        type: "Post",
        data: dataToPost,
        success: function (data) {
            if (data.HasError) {
                alert(data.Message);
            }
            else {
                window.location.href = "../Site/SiteDependencies/";
            }
        }
    });
    return true
});



$('#btnPotentialDependencySystem').click(function () {

    //added by barnali for part pick up 

    var SSOID1;
    var SSOID2;
    var SSOID3;
    var ID1;
    var ID2;
    var ID3;
    var fseSkill;
    var isrequiredfse;
    isrequiredfse = $("#chkfse1").is(":checked");
    fseSkill = $("#Skillfse1").val();
    var values = '';
    if ($("#hsrType").val() == "Installation") {
        if ($('.selectpicker').val() != null) {
            if ($('.selectpicker').val().indexOf("EnterSSOID") == "-1") {
                if ($('.editOption').val() == '') {
                    values = $(".selectpicker").val()
                }
                else {
                    if ($(".selectpicker").val() == null) {
                        values = $('.editOption').val();
                    } else {
                        values = $(".selectpicker").val() + $('.editOption').val();
                    }
                }
            } else {

                if ($('.editOption').val() == '') {
                    values = $(".selectpicker").val()
                    values = values.slice(0, -1);
                }
                else {
                    if ($(".selectpicker").val() == null) {
                        values = $('.editOption').val();
                    } else {
                        values = $(".selectpicker").val();
                        values = values.slice(0, -1) + ',' + $('.editOption').val();
                        if (values.substring(0, 1) == ",") {
                            values = values.split(',').slice(1);
                        }

                    }
                }


            }
        }
    }
    else {
        values = $(".selectpicker").val();
    }
    if (values != null) {
        SSOID1 = values.toString().split('||');
        ID1 = SSOID1[0]
    }
    else {
        ID1 = null;
    }
    var select = $("#SSoid3").val();
    if (select != null) {
        SSOID3 = select.toString().split('||');
        ID3 = SSOID3[0]
    }
    else {
        ID3 = null;
    }
    var val = $("#SSoid2").val();
    if (val != null) {
        SSOID2 = val.toString().split('||');
        ID2 = SSOID2[0]
    }
    else {
        ID2 = null;
    }
    var isDisabled = $('#txtDelivery').prop('disabled');

    if ($('#hsrType').val() == "Installation") {
        var IsInstallValidationsFail = taskDatetimeValidations()
        if (IsInstallValidationsFail)
            return true;
    }
    else {
        var AppWindow = $('#appWindow input:radio:checked');

        if ($('#hsrType').val() == "Corrective Repair" && $('#hCREntitlementFlag').val() == "Y" && $("#txtestart").val() == "" || $("#txtlstart").val() == "") {


        }
        else {
            var IsValidationFail = EarlyStartAndLateStartValidations();
            if (IsValidationFail)
                return true;
        }
    }
    if ($('#hsrType').val() == "Corrective Repair" && $('#hCREntitlementFlag').val() == "Y" && $("#txtestart").val() == "") {

    }
    else {
        if (!isDisabled) {
            var IsDeliveryDateValidationFail = DeliveryDateValidations()

            if (IsDeliveryDateValidationFail)
                return true;
        }
    }

    if ($('#hsrType').val() == "Installation") {
        if ($('.editOption').val() != "") {
            if (SSOIDinvalid == false) {
                jQuery('#modal-2').modal('show')
                $("#text").html("SSOID is Invalid. Please enter a valid SSOID");
                return;
            }
        }
    }

    if ($('#hsrType').val() == "Installation") {
        if ($("#htDurationInstall").val() == 0 || $("#htDurationInstall").val() == null) {
            $("#text").html("Task Duration can't be zero. Please enter...");
            jQuery('#modal-2').modal('show')
            $('#taskduration').focus();
            return true;
        }
    }


    if ($('#hsrType').val() != "Installation") {
        if ($("#taskduration").val() == 0 || $("#taskduration").val() == null) {
            jQuery('#modal-2').modal('show')

            $("#text").html("Visit Duration can't be zero. Please enter...")
            $('#taskduration').focus();
            return true;
        }

    }
    if (GetSiteSystemData.SystemCountPotential <= 0) {
        jQuery('#modal-2').modal('show')
        $("#text").html("No Potential System Dependency Job Found.")
        return true;
    }


    var deliverytype = $('#deliverytype :selected').text();

    var partComment = $("#txtPartComments").val();

    var deliveryDate = $("#txtDelivery").val();
    var address = $("#address").val();
    if (deliverytype != "" && deliverytype != "--Please Select--") {
        AddOrUpdateComment2(deliverytype, partComment, deliveryDate, address);
    }
    if (deliverytype != "" && deliverytype != "--Please Select--") {
        AddOrUpdateComment(deliverytype, partComment, deliveryDate, address);
    }

    var s = '';

    for (i = 0; i < addr2.length; i++) {

        for (j = 0; j < addr2[i].length; j++) {
            //No need to chnage change the Name
            if (addr2[i][j][4] == "South Korea") {

                addr2[i][j][4] = "KOREA, REPUBLIC OF";
            }
            //Before Modify Check whether Customer site selected or not
            if (j == 0)
                if (addr2[i].length == 1) {

                    s += '"' + addr2[i][j] + '"';
                }
                else
                    s += '"' + addr2[i][j] + '"' + '=';
            else
                if (j != (addr2[i].length - 1)) {
                    s += '"' + addr2[i][j] + '"' + '=';
                }
                else {
                    s += '"' + addr2[i][j] + '"';
                }

        }
    }
    s += ",";
    if (s == ",") {
        s = "";
    }
    var tasknotes = $('#tasknotes').val();
    if (tasknotes != "") {
        if (tasknotes.indexOf(',') != -1) {
            tasknotes = tasknotes.replace(/,/g, "");
        }
    }

    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
    var srdesc = $('#srdesc').val();
    if (srdesc != "") {
        if (srdesc.indexOf(',') != -1) {
            srdesc = srdesc.replace(/,/g, "");
        }
    }
    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends

    // Changes done by Raju, send array to controller intead of string

    var installDuration = ($("#htDurationInstall").val() * 60).toString();
    if ($("#hsrType").val() == "Installation") {
        var dataToPost = {
            TaskSystemID: $('#hTaskSystemID').val(),
            SystemCountActual: GetSiteSystemData.SystemCountActual,
            EStart: $("#txtestart").val(),
            LStart: "",
            Duration: installDuration,
            TaskNotes: tasknotes,
            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
            SRDesc: srdesc,
            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
            DependencyType: "PotentialDependency",
            addressArray: addr2,
            Profile: "AM/PM",
            SSOFse1: ID1,
            fseSkill: fseSkill,
            IsRequiredfse: isrequiredfse,
            JobType: $("#hsrType").val()

        };
    }
    else {
        var dataToPost = {
            TaskSystemID: $('#hTaskSystemID').val(),
            SystemCountActual: GetSiteSystemData.SystemCountActual,
            EStart: $("#txtestart").val(),
            LStart: $("#txtlstart").val(),
            Duration: $("#taskduration").val().toString(),
            TaskNotes: tasknotes,
            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
            SRDesc: srdesc,
            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
            DependencyType: "PotentialDependency",
            addressArray: addr2,
            Profile: AppWindow.attr('value'),
            SSOFse1: ID1,
            fseSkill: fseSkill,
            IsRequiredfse: isrequiredfse,
            JobType: $("#hsrType").val()

        };

    }
    $.ajax({
        url: '../System/SystemDependencies',
        type: "Post",
        data: dataToPost,
        success: function (data) {
            if (data.HasError) {
                alert(data.Message);
            }
            else {
                window.location.href = "../System/SystemDependencies/";
            }
        }
    });
    return true
});


$('#btnActualDependencySystem').click(function () {
    var SSOID1;
    var SSOID2;
    var SSOID3;
    var ID1;
    var ID2;
    var ID3;
    var fseSkill;
    var isrequiredfse;
    isrequiredfse = $("#chkfse1").is(":checked");
    fseSkill = $("#Skillfse1").val();
    var values = '';
    if ($("#hsrType").val() == "Installation") {
        if ($('.selectpicker').val() != null) {
            if ($('.selectpicker').val().indexOf("EnterSSOID") == "-1") {
                if ($('.editOption').val() == '') {
                    values = $(".selectpicker").val()
                }
                else {
                    if ($(".selectpicker").val() == null) {
                        values = $('.editOption').val();
                    } else {
                        values = $(".selectpicker").val() + $('.editOption').val();
                    }
                }
            } else {

                if ($('.editOption').val() == '') {
                    values = $(".selectpicker").val()
                    values = values.slice(0, -1);
                }
                else {
                    if ($(".selectpicker").val() == null) {
                        values = $('.editOption').val();
                    } else {
                        values = $(".selectpicker").val();
                        values = values.slice(0, -1) + ',' + $('.editOption').val();
                        if (values.substring(0, 1) == ",") {
                            values = values.split(',').slice(1);
                        }

                    }
                }


            }
        }
    }
    else {
        values = $(".selectpicker").val();
    }
    if (values != null) {
        SSOID1 = values.toString().split('||');
        ID1 = SSOID1[0]
    }
    else {
        ID1 = null;
    }
    var select = $("#SSoid3").val();
    if (select != null) {
        SSOID3 = select.toString().split('||');
        ID3 = SSOID3[0]
    }
    else {
        ID3 = null;
    }
    var val = $("#SSoid2").val();
    if (val != null) {
        SSOID2 = val.toString().split('||');
        ID2 = SSOID2[0]
    }
    else {
        ID2 = null;
    }
    var isDisabled = $('#txtDelivery').prop('disabled');

    if ($('#hsrType').val() == "Installation") {

    }
    else {
        var AppWindow = $('#appWindow input:radio:checked');


    }

    var deliverytype = $('#deliverytype :selected').text();

    var partComment = $("#txtPartComments").val();

    var deliveryDate = $("#txtDelivery").val();
    var address = $("#address").val();
    if (deliverytype != "" && deliverytype != "--Please Select--") {
        AddOrUpdateComment2(deliverytype, partComment, deliveryDate, address);
    }
    if (deliverytype != "" && deliverytype != "--Please Select--") {
        AddOrUpdateComment(deliverytype, partComment, deliveryDate, address);
    }

    var s = '';

    //

    for (i = 0; i < addr2.length; i++) {

        for (j = 0; j < addr2[i].length; j++) {
            //No need to chnage change the Name
            if (addr2[i][j][4] == "South Korea") {

                addr2[i][j][4] = "KOREA, REPUBLIC OF";
            }
            //Before Modify Check whether Customer site selected or not
            if (j == 0)
                if (addr2[i].length == 1) {

                    s += '"' + addr2[i][j] + '"';
                }
                else
                    s += '"' + addr2[i][j] + '"' + '=';
            else
                if (j != (addr2[i].length - 1)) {
                    s += '"' + addr2[i][j] + '"' + '=';
                }
                else {
                    s += '"' + addr2[i][j] + '"';
                }

        }
    }


    s += ",";
    if (s == ",") {
        s = "";
    }
    var tasknotes = $('#tasknotes').val();
    if (tasknotes != "") {

        if (tasknotes.indexOf(',') != -1) {
            tasknotes = tasknotes.replace(/,/g, "");
        }
    }

    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
    var srdesc = $('#srdesc').val();
    if (srdesc != "") {

        if (srdesc.indexOf(',') != -1) {
            srdesc = srdesc.replace(/,/g, "");
        }
    }
    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends


    // Changes done by Raju, send array to controller intead of string

    var installDuration = ($("#htDurationInstall").val() * 60).toString();

    if ($("#hsrType").val() == "Installation") {
        var dataToPost = {
            TaskSystemID: $('#hTaskSystemID').val(),
            EStart: $("#txtestart").val(),
            LStart: "",
            Duration: installDuration,
            TaskNotes: tasknotes,
            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
            SRDesc: srdesc,
            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
            DependencyType: "ActualDependency",
            addressArray: addr2,
            Profile: "AM/PM",
            SSOFse1: ID1,
            fseSkill: fseSkill,
            IsRequiredfse: isrequiredfse,
            JobType: $("#hsrType").val()
        };
    }
    else {
        var dataToPost = {
            TaskSystemID: $('#hTaskSystemID').val(),
            EStart: $("#txtestart").val(),
            LStart: $("#txtlstart").val(),
            Duration: $("#taskduration").val().toString(),
            TaskNotes: tasknotes,
            ///Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
            SRDesc: srdesc,
            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
            DependencyType: "ActualDependency",
            addressArray: addr2,
            Profile: AppWindow.attr('value'),
            SSOFse1: ID1,
            fseSkill: fseSkill,
            IsRequiredfse: isrequiredfse,
            JobType: $("#hsrType").val()

        };

    }
    $.ajax({
        url: '../System/SystemDependencies',
        type: "Post",
        data: dataToPost,
        success: function (data) {
            if (data.HasError) {
                alert(data.Message);
            }
            else {
                window.location.href = "../System/SystemDependencies/";
            }
        }
    });
    return true
});

function ModifyMainAndPartValidations() {
    
    var task = "Task Duration : " + $("#taskduration").val();
    //Added code taskduration(days+hrs) for installation job
    if ($('#hsrType').val() == "Installation") {
        task = "Task Duration : " + $("#htDurationInstall").val();
    }
    var sart = "";
    var late = "";
    if ($('#hsrType').val() == "Installation") {
        sart = "Task Start Date Time : " + $("#txtestart").val();
    }
    else {
        sart = "Early Start : " + $("#txtestart").val();
        late = "Late Start : " + $("#txtlstart").val();
    }


    if (fseskills != $("#Skillfse1").val()) {
        fseskills1 = "Skill Level : " + $("#Skillfse1 option:selected").html();
        if ($("#Skillfse1 option:selected").html() == 'None') {
            fseskills1 = "";
        }
    } else {
        fseskills1 = "";
    }


    var tNotes = "Task Notes : " + $("#tasknotes").val();
    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
    var srdescription = "SR Description : " + $("#srdesc").val();
    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends

    var ssoid;
    if (myselect != null)
    { ssoid = "SSOID : " + myselect; }
    else {
        ssoid = "";
    }
    
    var alertMessage = "Do You Really Want To Modify This Task<br>";

    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
    if (gduration != $("#taskduration").val() && gES != $("#txtestart").val() && gLS != $("#txtlstart").val() && gSRDesc != $("#srdesc").val()) {
        jQuery('#modal-3').modal('show');
        if (gTaskNotes == $("#tasknotes").val())
            $("#modifypop").html(alertMessage + srdescription + "</br>" + task + "</br>" + sart + "<br>" + late + "<br>" + ssoid + "</br>" + fseskills1);
        else
            $("#modifypop").html(alertMessage + srdescription + "</br>" + task + "</br>" + sart + "<br>" + late + "</br>" + tNotes + "<br>" + ssoid + "</br>" + fseskills1);
    }

    else if (gduration != $("#taskduration").val() && gES != $("#txtestart").val() && gLS != $("#txtlstart").val() && gSRDesc == $("#srdesc").val()) {
        jQuery('#modal-3').modal('show');
        if (gTaskNotes == $("#tasknotes").val())
            $("#modifypop").html(alertMessage + task + "</br>" + sart + "<br>" + late + "<br>" + ssoid + "</br>" + fseskills1);
        else
            $("#modifypop").html(alertMessage + task + "</br>" + sart + "<br>" + late + "</br>" + tNotes + "<br>" + ssoid + "</br>" + fseskills1);
    }

    else if (gduration != $("#taskduration").val() && gES != $("#txtestart").val() && gLS == $("#txtlstart").val() && gSRDesc != $("#srdesc").val()) {
        jQuery('#modal-3').modal('show');
        if (gTaskNotes == $("#tasknotes").val())
            $("#modifypop").html(alertMessage + srdescription + "</br>" + task + "</br>" + sart + "<br>" + ssoid + "</br>" + fseskills1);
        else
            $("#modifypop").html(alertMessage + srdescription + "</br>" + task + "</br>" + sart + "<br>" + tNotes + "<br>" + ssoid + "</br>" + fseskills1);
    }

    else if (gduration != $("#taskduration").val() && gES != $("#txtestart").val() && gLS == $("#txtlstart").val() && gSRDesc == $("#srdesc").val()) {
        jQuery('#modal-3').modal('show');
        if (gTaskNotes == $("#tasknotes").val())
            $("#modifypop").html(alertMessage + task + "</br>" + sart + "<br>" + ssoid + "</br>" + fseskills1);
        else
            $("#modifypop").html(alertMessage + task + "</br>" + sart + "<br>" + tNotes + "<br>" + ssoid + "</br>" + fseskills1);
    }

    else if (gduration != $("#taskduration").val() && gES == $("#txtestart").val() && gLS != $("#txtlstart").val() && gSRDesc != $("#srdesc").val()) {
        jQuery('#modal-3').modal('show');
        if (gTaskNotes == $("#tasknotes").val())
            $("#modifypop").html(alertMessage + srdescription + "</br>" + task + "</br>" + late + "<br>" + ssoid + "</br>" + fseskills1);
        else
            $("#modifypop").html(alertMessage + srdescription + "</br>" + task + "</br>" + late + "</br>" + tNotes + "<br>" + ssoid + "</br>" + fseskills1);
    }

    else if (gduration != $("#taskduration").val() && gES == $("#txtestart").val() && gLS != $("#txtlstart").val() && gSRDesc == $("#srdesc").val()) {
        jQuery('#modal-3').modal('show');
        if (gTaskNotes == $("#tasknotes").val())
            $("#modifypop").html(alertMessage + task + "</br>" +  late + "<br>" + ssoid + "</br>" + fseskills1);
        else
            $("#modifypop").html(alertMessage + task + "</br>" +  late + "</br>" + tNotes + "<br>" + ssoid + "</br>" + fseskills1);
    }

    else if (gduration != $("#taskduration").val() && gES == $("#txtestart").val() && gLS == $("#txtlstart").val() && gSRDesc != $("#srdesc").val()) {
        jQuery('#modal-3').modal('show');
        if (gTaskNotes == $("#tasknotes").val())
            $("#modifypop").html(alertMessage + srdescription + "</br>" + task + "</br>" + ssoid + "</br>" + fseskills1);
        else
            $("#modifypop").html(alertMessage + srdescription + "</br>" + task + "</br>" + tNotes + "<br>" + ssoid + "</br>" + fseskills1);
    }

    else if (gduration != $("#taskduration").val() && gES == $("#txtestart").val() && gLS == $("#txtlstart").val() && gSRDesc == $("#srdesc").val()) {
        jQuery('#modal-3').modal('show');
        if (gTaskNotes == $("#tasknotes").val())
            $("#modifypop").html(alertMessage + task + "</br>" + ssoid + "</br>" + fseskills1);
        else
            $("#modifypop").html(alertMessage + task + "</br>" + tNotes + "<br>" + ssoid + "</br>" + fseskills1);
    }
    else if (gduration == $("#taskduration").val() && gES != $("#txtestart").val() && gLS != $("#txtlstart").val() && gSRDesc != $("#srdesc").val()) {
        jQuery('#modal-3').modal('show');
        if (gTaskNotes == $("#tasknotes").val())
            $("#modifypop").html(alertMessage + srdescription + "</br>" + sart + "<br>" + late + "<br>" + ssoid + "</br>" + fseskills1);
        else
            $("#modifypop").html(alertMessage + srdescription + "</br>" + sart + "<br>" + late + "</br>" + tNotes + "<br>" + ssoid + "</br>" + fseskills1);
    }
    else if (gduration == $("#taskduration").val() && gES != $("#txtestart").val() && gLS != $("#txtlstart").val() && gSRDesc == $("#srdesc").val()) {
        jQuery('#modal-3').modal('show');
        if (gTaskNotes == $("#tasknotes").val())
            $("#modifypop").html(alertMessage + sart + "<br>" + late + "<br>" + ssoid + "</br>" + fseskills1);
        else
            $("#modifypop").html(alertMessage + sart + "<br>" + late + "</br>" + tNotes + "<br>" + ssoid + "</br>" + fseskills1);
    }
    else if (gduration == $("#taskduration").val() && gES != $("#txtestart").val() && gLS == $("#txtlstart").val() && gSRDesc != $("#srdesc").val()) {
        jQuery('#modal-3').modal('show');
        if (gTaskNotes == $("#tasknotes").val())
            $("#modifypop").html(alertMessage + srdescription + "</br>" + sart + "<br>" + ssoid + "</br>" + fseskills1);
        else
            $("#modifypop").html(alertMessage + srdescription + "</br>" + sart + "<br>" + tNotes + "<br>" + ssoid + "</br>" + fseskills1);
    }
    else if (gduration == $("#taskduration").val() && gES != $("#txtestart").val() && gLS == $("#txtlstart").val() && gSRDesc == $("#srdesc").val()) {
        jQuery('#modal-3').modal('show');
        if (gTaskNotes == $("#tasknotes").val())
            $("#modifypop").html(alertMessage + sart + "<br>" + ssoid + "</br>" + fseskills1);
        else
            $("#modifypop").html(alertMessage + sart + "<br>" + tNotes + "<br>" + ssoid + "</br>" + fseskills1);
    }
    else if (gduration == $("#taskduration").val() && gES == $("#txtestart").val() && gLS != $("#txtlstart").val() && gSRDesc != $("#srdesc").val()) {
        jQuery('#modal-3').modal('show');
        if (gTaskNotes == $("#tasknotes").val())
            $("#modifypop").html(alertMessage + srdescription + "</br>" + late + "<br>" + ssoid + "</br>" + fseskills1);
        else
            $("#modifypop").html(alertMessage + srdescription + "</br>" + late + "</br>" + tNotes + "<br>" + ssoid + "</br>" + fseskills1);
    }

    else if (gduration == $("#taskduration").val() && gES == $("#txtestart").val() && gLS != $("#txtlstart").val() && gSRDesc == $("#srdesc").val()) {
        jQuery('#modal-3').modal('show');
        if (gTaskNotes == $("#tasknotes").val())
            $("#modifypop").html(alertMessage + late + "<br>" + ssoid + "</br>" + fseskills1);
        else
            $("#modifypop").html(alertMessage + late + "</br>" + tNotes + "<br>" + ssoid + "</br>" + fseskills1);
    }
    else if (gduration == $("#taskduration").val() && gES == $("#txtestart").val() && gLS == $("#txtlstart").val() && gSRDesc != $("#srdesc").val()) {
        jQuery('#modal-3').modal('show');
        if (gTaskNotes == $("#tasknotes").val())
            $("#modifypop").html(alertMessage + srdescription + "</br>" + ssoid + "</br>" + fseskills1);
        else
            $("#modifypop").html(alertMessage + srdescription + "</br>" + tNotes + "<br>" + ssoid + "</br>" + fseskills1);
    }

    else if (gduration == $("#taskduration").val() && gES == $("#txtestart").val() && gLS == $("#txtlstart").val() && gSRDesc == $("#srdesc").val() && SSOID1Validate == true) {
        jQuery('#modal-3').modal('show');
        if (gTaskNotes == $("#tasknotes").val())
            $("#modifypop").html(alertMessage + ssoid + "</br>" + fseskills1);
        else
            $("#modifypop").html(alertMessage + tNotes + "</br>" + ssoid + "</br>" + fseskills1);
    }

    else if (fseskills != $("#Skillfse1").val() && gduration == $("#taskduration").val() && gES == $("#txtestart").val() && gLS == $("#txtlstart").val() && gSRDesc == $("#srdesc").val() && SSOID1Validate != true) {

        if (gTaskNotes == $("#tasknotes").val()) {
            jQuery('#modal-3').modal('show');
            $("#modifypop").html(alertMessage + fseskills1);
        }
        else {
            jQuery('#modal-3').modal('show');
            $("#modifypop").html(alertMessage + task + "</br>" + fseskills1);
        }
    }
    else if (gTaskNotes != $("#tasknotes").val()) {
        jQuery('#modal-3').modal('show');
        $("#modifypop").html(alertMessage + tNotes + "<br>" + ssoid + "</br>" + fseskills1);
    }
    else {

        jQuery('#modal-3').modal('hide');
        jQuery('#modal-modifysitesystem').modal('hide');
        $('#modifytask').click();

    }
    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends

}

/* Part pickup modify popup for YES*/
$("#modifyTaskModel").click(function (e) {
    var actvityDep = $("#checktaskid").val();
    if (GetSiteSystemData.SiteCountActual > 0 || GetSiteSystemData.SystemCountActual > 0) {
        SiteCountActualflag = true;
        jQuery('#modal-modifysitesystem').modal('show');
        var alertMessageDep = "There is a dependency job exist with " + actvityDep + ", please confirm whether you want to modify appointments for this job ? " + '<br>';

        jQuery('#modal-1').modal('hide');
        jQuery('#modal-modifyPart1').modal('hide')
        $("#modifypopsitesystem").html(alertMessageDep);
        return;
    } else {
        jQuery('#modal-modifyPart1').modal('hide')

        ModifyMainAndPartValidations();
    }
});


$("#modVis").click(function (e) {
    var isrequiredfse;
    isrequiredfse = $("#chkfse1").is(":checked");

    if ($('#hsrType').val() == "Installation") {
        if ($('.editOption').val() != "") {
            if (SSOIDinvalid == false) {
                jQuery('#modal-2').modal('show')
                $("#text").html("SSOID is Invalid. Please enter a valid SSOID");
                return;
            }
        }
    }


    if ($('#hsrType').val() == "Installation") {
        if ($("#htDurationInstall").val() == 0 || $("#htDurationInstall").val() == null) {
            $("#text").html("Task Duration can't be zero. Please enter...");
            jQuery('#modal-2').modal('show')
            $('#taskduration').focus();
            return true;
        }
    }


    if ($('#hsrType').val() != "Installation") {
        if ($("#taskduration").val() == 0 || $("#taskduration").val() == null) {
            jQuery('#modal-2').modal('show')

            $("#text").html("Visit Duration can't be zero. Please enter...")
            $('#taskduration').focus();
            return true;
        }

    }
    var isDisabled = $('#txtDelivery').prop('disabled');

    if ($('#hsrType').val() == "Installation") {
        var IsInstallValidationsFail = taskDatetimeValidations()
        if (IsInstallValidationsFail)
            return true;
    }
    else {
        if ($('#hsrType').val() == "Corrective Repair" && $('#hCREntitlementFlag').val() == "Y" && $("#txtestart").val() == "" || $("#txtlstart").val() == "") {


        }
        else {
            var IsValidationFail = EarlyStartAndLateStartValidations();
            if (IsValidationFail)
                return true;
        }
    }
    if ($('#hsrType').val() == "Corrective Repair" && $('#hCREntitlementFlag').val() == "Y" && $("#txtestart").val() == "") {


    }
    else {
        if (!isDisabled) {
            var IsDeliveryDateValidationFail = DeliveryDateValidations()

            if (IsDeliveryDateValidationFail)
                return true;
        }
    }

    var task = "Task Duration : " + $("#taskduration").val();
    //Added code taskduration(days+hrs) for installation job
    if ($('#hsrType').val() == "Installation") {
        task = "Task Duration : " + $("#htDurationInstall").val();
    }
    var sart = "";
    if ($('#hsrType').val() == "Installation") {
        sart = "Task Start Date Time : " + $("#txtestart").val();
    }
    else {
        sart = "Early Start : " + $("#txtestart").val();
    }

    if (fseskills != $("#Skillfse1").val()) {
        if ($("#Skillfse1").val() != 0) {
            fseskills1 = "Skill Level : " + "Level " + $("#Skillfse1").val();
        } else {
            fseskills1 = "Skill Level : " + "Any FE";
        }

    } else {
        fseskills1 = "";
    }


    var late = "Late Start : " + $("#txtlstart").val();
    var tNotes = "Task Notes : " + $("#tasknotes").val();
    var ssoid;
    if (myselect != null)
    { ssoid = "SSOID : " + myselect; }
    else {
        ssoid = "";
    }

    var countData = json.length;
    var partjobExitflag = false;
    if (countData > 0) {
        for (var i = 0; i < json.length; i++) {
            if (json[i].Status != "Cancelled") {

                partjobExitflag = true;
                jQuery('#modal-3').modal('hide');
                var alertMessageDep = "Part Pick up job exists. Do you want to proceed with modify of both main and part pick-up job? " + '<br>';
                jQuery('#modal-modifyPart1').modal('show')
                $("#modifyPartPopup").html(alertMessageDep);
                return;
            }
            else {
                jQuery('#modal-modifyPart1').modal('hide')
            }
        }

    }
    else {
        jQuery('#modal-modifyPart1').modal('hide')
    }

    if (partjobExitflag != true) {
        if (GetSiteSystemData.SiteCountActual > 0 || GetSiteSystemData.SystemCountActual > 0) {
            var actvityDep = $("#checktaskid").val();
            SiteCountActualflag = true;
            jQuery('#modal-modifysitesystem').modal('show');

            var alertMessageDep = "There is a dependency job exist with " + actvityDep + ", please confirm whether you want to modify appointments for this job ? " + '<br>';
            jQuery('#modal-1').modal('hide');
            $("#modifypopsitesystem").html(alertMessageDep);
            return;
        }
    }
    var alertMessage = "Do You Really Want To Modify This Task<br>";
    if (partjobExitflag != true) {
        ModifyMainAndPartValidations();
    }
})
var depPartFlag = false;
var depSysFlag = false;
var SystemStat, actvityDetails, SiteStat = "";

$("#btnCancelTask").click(function (e) {



    var btnSite = 0; var btnSystem = 0;
    if (GetSiteSystemData != undefined && GetSiteSystemData.SiteCountActual != undefined) {
        btnSite = GetSiteSystemData.SiteCountActual;
    }
    if (GetSiteSystemData != undefined && GetSiteSystemData.SystemCountActual != undefined) {
        btnSystem = GetSiteSystemData.SystemCountActual;
    }


    var actvityDep = $("#checktaskid").val();
    var countData = json.length;


    if (countData > 0) {

        var alertMessageDep = "Part Pick up job exists. Do you want to proceed with cancellation of both main and part pick-up job? " + '<br>';
        depPartFlag = true;
        jQuery('#modal-Part1').modal('show');
        jQuery('#modal-1').modal('hide');
        $("#cancelPartPopup").html(alertMessageDep);


    }
    else {
        depPartFlag = false;
    }

    var CallId = $('#callIDdata').val($('#hcalltestiddata').val())[0].value;
    if ((btnSite != 0 || btnSystem != 0) && ($("#SDTrequestAppointmentStartDte").val() != "" || $("#SDTrequestAppointmentStartDte").val() != undefined)) {

        var activityId = "APAC-" + actvityDep + "-1";
        if (activityId == CallId) {
            var alertMessageDep = "Dependency Job exists, Please de-link the Activity #" + actvityDep + " in SDT Schedule." + '<br>' + " Do you want to proceed with cancellation of both main and dependency(s) job? " + '<br>';
        } else {
            var alertMessageDep = "Dependency Job exists. Do you want to proceed with cancellation of Job# " + actvityDep + " ?" + '<br>';
        }
        depSysFlag = true;

        jQuery('#modal-Depnedency').modal('show');
        jQuery('#modal-1').modal('hide');
        $("#cancelPopup").html(alertMessageDep);
    }
    else {
        depSysFlag = false;
    }

    if (depPartFlag == false && depSysFlag == false) { jQuery('#modal-1').modal('show'); }

})

$("#canclTaskModel").click(function (e) {

    if (depSysFlag == true) {
        jQuery('#modal-Depnedency').modal('hide');
        depSysFlag = false;
    }

    if (depPartFlag == true) {

    }
    else { jQuery('#modal-1').modal('show'); }


})


$("#canclPTaskModel").click(function (e) {
    if (depPartFlag == true) {
        jQuery('#modal-Part1').modal('hide');
        depPartFlag = false;

    }
    if (depSysFlag == true) {

    }
    else {
        jQuery('#modal-1').modal('show');
    }

})
/*Modify Site System Dependency Popup's*/

$('#ModifyYesDepTask').click(function () {


    jQuery('#modal-modifyPart1').modal('hide')
    jQuery('#modal-modifysitesystem').modal('hide');
    ModifyMainAndPartValidations();
});
/*Modify Site System Dependency Popup's*/


$("#modifytask").click(function (e) {
    var isrequiredfse;
    isrequiredfse = $("#chkfse1").is(":checked");
    jQuery('#modal-3').modal('hide');


    var ID1;

    var values = '';
    if ($("#hsrType").val() == "Installation") {
        if ($('.selectpicker').val() != null) {
            if ($('.selectpicker').val().indexOf("EnterSSOID") == "-1") {
                if ($('.editOption').val() == '') {
                    values = $(".selectpicker").val()
                }
                else {
                    if ($(".selectpicker").val() == null) {
                        values = $('.editOption').val();
                    } else {
                        values = $(".selectpicker").val() + $('.editOption').val();
                    }
                }
            } else {

                if ($('.editOption').val() == '') {
                    values = $(".selectpicker").val()
                    values = values.slice(0, -1);
                }
                else {
                    if ($(".selectpicker").val() == null) {
                        values = $('.editOption').val();
                    } else {
                        values = $(".selectpicker").val();
                        values = values.slice(0, -1) + ',' + $('.editOption').val();
                        if (values.substring(0, 1) == ",") {
                            values = values.split(',').slice(1);
                        }

                    }
                }


            }
        }
    } else {
        values = $(".selectpicker").val();
    }
    if (values != null) {
        SSOID1 = values.toString().split('||');
        ID1 = SSOID1[0]
    }
    else {
        ID1 = null;
    }
    var fseSkill = $("#Skillfse1").val() + "," + $("#Skillfse2").val() + "," + $("#Skillfse3").val();
    var s = '';

    var deliverytype = $('#deliverytype :selected').text();

    var partComment = $("#txtPartComments").val();

    var deliveryDate = $("#txtDelivery").val();
    var address = $("#address").val();
    if (deliverytype != "" && deliverytype != "--Please Select--") {
        AddOrUpdateComment2(deliverytype, partComment, deliveryDate, address);
    }
    if (deliverytype != "" && deliverytype != "--Please Select--") {
        AddOrUpdateComment(deliverytype, partComment, deliveryDate, address);
    }
    var flg = false;

    for (i = 0; i < addr2.length; i++) {

        for (j = 0; j < addr2[i].length; j++) {
            //No need to chnage change the Name
            if (addr2[i][j][4] == "South Korea") {

                addr2[i][j][4] = "KOREA, REPUBLIC OF";
            }
            //Before Modify Check whether Customer site selected or not
            if (j == 0)
                if (addr2[i].length == 1) {

                    s += '"' + addr2[i][j] + '"';
                }
                else
                    s += '"' + addr2[i][j] + '"' + '=';
            else
                if (j != (addr2[i].length - 1)) {
                    s += '"' + addr2[i][j] + '"' + '=';
                }
                else {
                    s += '"' + addr2[i][j] + '"';
                }

        }
    }
    if (addr)
        s += ",";
    var tasknotes = $('#tasknotes').val();

    if (tasknotes != "") {

        if (tasknotes.indexOf(',') != -1) {
            tasknotes = tasknotes.replace(/,/g, "");
        }
    }

    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
    var srdesc = $('#srdesc').val();   
    //if (srdesc != "") {

    //    if (srdesc.indexOf(',') != -1) {
    //        srdesc = srdesc.replace(/,/g, "");
    //    }
    //}
    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends


    e.preventDefault();
    // Changes done by Raju, send array to controller intead of string

 

    if ($("#hsrType").val() == "Installation") {
        var dataToPost = {

            earlyStart: $("#txtestart").val(),
            lateStart: "",
            duration: $("#htDurationInstall").val().toString(),
            addressArray: addr2,
            TaskNotes: tasknotes,                    
            fseSkill: $("#Skillfse1").val() + "," + $("#Skillfse2").val() + "," + $("#Skillfse3").val(),
            PreferredFSEs: ID1,
            JobType: "Install",
            partcomments: $('#txtPartComments').val(),
            IsRequiredfse: isrequiredfse,
            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
            SRDesc: srdesc
            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
        };

    }
    else {
        var dataToPost = {
            earlyStart: $("#txtestart").val(),
            lateStart: $("#txtlstart").val(),
            duration: $("#taskduration").val().toString(),
            addressArray: addr2,
            TaskNotes: tasknotes,                     
            fseSkill: $("#Skillfse1").val() + "," + $("#Skillfse2").val() + "," + $("#Skillfse3").val(),
            PreferredFSEs: ID1,
            JobType: "",
            partcomments: $('#txtPartComments').val(),
            IsRequiredfse: isrequiredfse,
            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
            SRDesc: srdesc
            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
           
        };
    }


    $.ajax({
        url: '../RequestAppointmentBooking/ModifyVisit',
        type: "POST",
        data: dataToPost,
        success: function (data) {

            if (data.message == "Success") {
                //modal reload added by Phani Kanth P.
                jQuery('#modal-Reload').modal('show')
                if (data.JobType == "Part") {

                    if (PartDataModified == true) {
                        $("#TextReload").html("Task # " + data.TaskID + " " + "Modified Successfully and Part Pick Up Job have been modified successfully.")
                    } else {
                        $("#TextReload").html("Task # " + data.TaskID + " " + "Modified Successfully.")
                    }
                }
            }
            else {
                if (data.Message == "Modified") {
                    //modal reload added by Phani Kanth P.
                    jQuery('#modal-Reload').modal('show')
                    $("#TextReload").html("Task Modified Successfully.")
                }
                else {
                    //jQuery('#modal-2').modal('show')
                    //$("#text").html("Task Not Modified. (Exception: " + data.Message + ")")
                    //Start Code - US139 - Added by Ebaad - 19/4/2017 - User Friendly Error Message pop-up when error occurs while modifying task
                    $("#apptext").html("An error occurred while modifying the Task !");
                    $("#reqapp-moredetails").append("Exception: " + data.message);
                    $("#reqapp-moredetails").hide();
                    jQuery('#modal-moredetails').modal('show');

                    $("#appmoredetails").click(function () {
                        $("#reqapp-moredetails").toggle();
                        $("#applessdetailsDiv").show();
                        $("#appmoredetailsDiv").hide();
                    });

                    $("#applessdetails").click(function () {
                        $("#reqapp-moredetails").toggle();
                        $("#appmoredetailsDiv").show();
                        $("#applessdetailsDiv").hide();
                    });
					//End Code - US139 - Added by Ebaad - 19/4/2017 - User Friendly Error Message pop-up when error occurs while modifying task
                }
            }
        },
        error: function () {
            //jQuery('#modal-2').modal('show')
            //$("#text").html("An error has occured while modifing the task.")
            //Start Code - US139 - Added by Ebaad - 19/4/2017 - User Friendly Error Message pop-up when error occurs while creating task
            $("#apptext").html("An error occurred while modifying the Task !");
            $("#reqapp-moredetails").append("Exception: " + data.message);
            $("#reqapp-moredetails").hide();
            jQuery('#modal-moredetails').modal('show');

            $("#appmoredetails").click(function () {
                $("#reqapp-moredetails").toggle();
                $("#applessdetailsDiv").show();
                $("#appmoredetailsDiv").hide();
            });

            $("#applessdetails").click(function () {
                $("#reqapp-moredetails").toggle();
                $("#appmoredetailsDiv").show();
                $("#applessdetailsDiv").hide();
            });
            //End Code - US139 - Added by Ebaad - 19/4/2017 - User Friendly Error Message pop-up when error occurs while creating task
        }
    });
    return true
})


$("#creVisNA").click(function (e) {

    if ($('#hsrType').val() == "Installation") {

        if ($('.editOption').val() != "") {
            if (SSOIDinvalid == false) {
                jQuery('#modal-2').modal('show')
                $("#text").html("SSOID is Invalid. Please enter a valid SSOID");
                return;
            }
        }
    }
    var isDisabled = $('#txtDelivery').prop('disabled');

    if ($('#hsrType').val() == "Installation") {
        var IsInstallValidationsFail = taskDatetimeValidations()
        if (IsInstallValidationsFail)
            return true;
    }
    else {
        if ($('#hsrType').val() == "Corrective Repair" && $('#hCREntitlementFlag').val() == "Y" && $("#txtestart").val() == "" || $("#txtlstart").val() == "") {


        }
        else {
            var IsValidationFail = EarlyStartAndLateStartValidations();
            if (IsValidationFail)
                return true;
        }
    }


    if ($('#hsrType').val() == "Corrective Repair" && $('#hCREntitlementFlag').val() == "Y" && $("#txtestart").val() == "") {


    }
    else {
        if (!isDisabled) {
            var IsDeliveryDateValidationFail = DeliveryDateValidations()

            if (IsDeliveryDateValidationFail)
                return true;
        }
    }




    if ($('#hsrType').val() == "Installation") {
        if ($("#htDurationInstall").val() == 0 || $("#htDurationInstall").val() == null) {
            $("#text").html("Task Duration can't be zero. Please enter...");
            jQuery('#modal-2').modal('show')
            $('#taskduration').focus();
            return true;
        }
    }


    if ($('#hsrType').val() != "Installation") {
        if ($("#taskduration").val() == 0 || $("#taskduration").val() == null) {
            jQuery('#modal-2').modal('show')

            $("#text").html("Visit Duration can't be zero. Please enter...")
            $('#taskduration').focus();
            return true;
        }

    }

    var fseSkill = $("#Skillfse1").val();
    //Comment by Phani Kanth because of skill 2 & skill 3 are disabled

    var s = '';
    var ID1;
    var SSOID1;

    var isrequiredfse;
    isrequiredfse = $("#chkfse1").is(":checked");

    var values = '';
    if ($("#hsrType").val() == "Installation") {
        if ($('.selectpicker').val() != null) {
            if ($('.selectpicker').val().indexOf("EnterSSOID") == "-1") {
                if ($('.editOption').val() == '') {
                    values = $(".selectpicker").val()
                }
                else {
                    if ($(".selectpicker").val() == null) {
                        values = $('.editOption').val();
                    } else {
                        values = $(".selectpicker").val() + $('.editOption').val();
                    }
                }
            } else {

                if ($('.editOption').val() == '') {
                    values = $(".selectpicker").val()
                    values = values.slice(0, -1);
                }
                else {
                    if ($(".selectpicker").val() == null) {
                        values = $('.editOption').val();
                    } else {
                        values = $(".selectpicker").val();
                        values = values.slice(0, -1) + ',' + $('.editOption').val();
                        if (values.substring(0, 1) == ",") {
                            values = values.split(',').slice(1);
                        }

                    }
                }


            }
        }
    } else {
        values = $(".selectpicker").val();
    }

    if (values != null) {
        SSOID1 = values.toString().split('||');
        ID1 = SSOID1[0]
    }
    else {
        ID1 = null;
    }
    var flg = false;
    var deliverytype = $('#deliverytype :selected').text();

    var partComment = $("#txtPartComments").val();

    var deliveryDate = $("#txtDelivery").val();
    var address = $("#address").val();
    if (deliverytype != "" && deliverytype != "--Please Select--") {
        AddOrUpdateComment2(deliverytype, partComment, deliveryDate, address);
    }
    if (deliverytype != "" && deliverytype != "--Please Select--") {
        AddOrUpdateComment(deliverytype, partComment, deliveryDate, address);
    }

    for (i = 0; i < addr1.length; i++) {

        for (j = 0; j < addr1[i].length; j++) {
            //No need to chnage change the Name
            if (addr1[i][j][4] == "South Korea") {

                addr1[i][j][4] = "KOREA, REPUBLIC OF";
            }
            //Before Create Check whether Customer site selected or not
            if (j == 0)
                if (addr1[i].length == 1) {
                    s += '"' + addr1[i][j] + '"';
                }
                else
                    s += '"' + addr1[i][j] + '"' + '=';
            else
                if (j != (addr1[i].length - 1)) {
                    s += '"' + addr1[i][j] + '"' + '=';
                }
                else {
                    s += '"' + addr1[i][j] + '"';
                }

        }
    }

    s += ",";
    var tasknotes = $('#tasknotes').val();

    if (tasknotes != "") {

        if (tasknotes.indexOf(',') != -1) {
            tasknotes = tasknotes.replace(/,/g, "");
        }
    }



  
    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
    var srdesc = $('#srdesc').val();

    //if (srdesc != "") {

    //    if (srdesc.indexOf(',') != -1) {
    //        srdesc = srdesc.replace(/,/g, "");
    //    }
    //}
    //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends

    // Changes done by Raju, send array to controller intead of string
    
  

    if ($("#hsrType").val() == "Installation") {
        var dataToPost = {
            Duration: $("#htDurationInstall").val(),
            TaskNotes: tasknotes,          
            addressArray: addr1,
            fseSkill: fseSkill,
            PreferredFSEs: ID1,
            partcomments: $('#txtPartComments').val(),
            taskOpenDate: $("#txtestart").val(),
            JobType: "Install",
            IsRequiredfse: isrequiredfse,
            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
            SRDesc: srdesc
            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
        };

    }
    else {
        var dataToPost = {
            EStart: $("#txtestart").val(),
            LStart: $("#txtlstart").val(),
            Duration: $("#taskduration").val().toString(),
            TaskNotes: tasknotes,            
            addressArray: addr1,
            fseSkill: fseSkill,
            PreferredFSEs: ID1,
            partcomments: $('#txtPartComments').val(),
            IsRequiredfse: isrequiredfse,
            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Begins
            SRDesc: srdesc
            //Jayesh Soni - US82 - 18/04/2017 - SR Description - Ends
        };
    }
    //
    e.preventDefault(); // <------------------ stop default behaviour of button
    var element = this;

    $.ajax({
        url: '../RequestAppointmentBooking/ProcessTaskwithoutAppointment',
        type: "POST",
        data: dataToPost,
        success: function (data) {
            if (data.message == "Success" && data.JobType == "Part") {
                jQuery('#modal-Reload').modal('show')
                $("#TextReload").html(" Task # " + data.TaskID + " and Part Pick Up Job have been created successfully.")//Changes By ankur
                $("#btnCancelTask").enabled = true;
                $("#modVis").visible = true;
                $("#creVisNA").visible = false;
                //<------------ submit form
            }
            else {
                if (data.message == "Success") {
                    jQuery('#modal-Reload').modal('show')
                    $("#TextReload").html("Task No: " + data.TaskID + " has been created successfully.");
                    $("#btnCancelTask").enabled = true;
                    $("#modVis").visible = true;
                    $("#creVisNA").visible = false;

                } //Code Start - Tejashree - 13/04/2017 - Scope: US186 SystemID is NULL
                else if ($("#ValidateSystemId").val() == "SystemIDNotExistInSiebel") {
                    jQuery('#modal-2').modal('show')
                    $("#text").html("Task cannot be created because SystemID is NULL")
                }//Code End - Tejashree - 13/04/2017 - Scope: US186 SystemID is NULL
                else {
                    //jQuery('#modal-2').modal('show')
                    //$("#text").html("Error occurs while creating the Task! (Exception:" + data.message + ")")
                    //US139 - Added by Ebaad - 19/4/2017 - User Friendly Error Message pop-up when error occurs while creating task
                    $("#apptext").html("An error occurred while creating the Task !");
                    $("#reqapp-moredetails").append("Exception: " + data.message);
                    $("#reqapp-moredetails").hide();
                    jQuery('#modal-moredetails').modal('show');

                    $("#reqapp-moredetails").click(function () {
                        $("#reqapp-moredetails").toggle();
                        $("#applessdetailsDiv").show();
                        $("#appmoredetailsDiv").hide();
                    });

                    $("#reqapp-lessdetails").click(function () {
                        $("#reqapp-moredetails").toggle();
                        $("#appmoredetailsDiv").show();
                        $("#applessdetailsDiv").hide();
                    });
                }
            }

        },
        error: function () {
            //jQuery('#modal-2').modal('show')
            //$("#text").html("An error has occured!!!")
            //US139 - Added by Ebaad - 19/4/2017 - User Friendly Error Message pop-up when error occurs while creating task
            $("#apptext").html("An error occurred while creating the Task !");
            $("#reqapp-moredetails").append("Exception: " + data.message);
            $("#reqapp-moredetails").hide();
            jQuery('#modal-moredetails').modal('show');

            $("#reqapp-moredetails").click(function () {
                $("#reqapp-moredetails").toggle();
                $("#applessdetailsDiv").show();
                $("#appmoredetailsDiv").hide();
            });

            $("#reqapp-lessdetails").click(function () {
                $("#reqapp-moredetails").toggle();
                $("#appmoredetailsDiv").show();
                $("#applessdetailsDiv").hide();
            });
        }
    });
});

$("#ReloadonClick").click(function (e) {

    location.reload();
})
$("#txtlstart").keyup(function (e) {


    if (e.which == 8 || e.which == 46) {
        if ($("#hTaskExistsStatus").val() == "Exists") {
        }
        else {
            if ($('#hsrType').val() == "Corrective Repair" && $('#hCREntitlementFlag').val() == "Y" && $("#txtlstart").val() == "") {

                $('#txtlstart').on('cancel.daterangepicker', function (ev, picker) {
                    //do something, like clearing an input
                    $(this).val('');
                });
                return false;
            }

        }
    }
});
$("#txtestart").keyup(function (e) {


    if (e.which == 8 || e.which == 46) {
        if ($("#hTaskExistsStatus").val() == "Exists") {
        }
        else {
            if ($('#hsrType').val() == "Corrective Repair" && $('#hCREntitlementFlag').val() == "Y" && $("#txtestart").val() == "") {

                $('#txtestart').on('cancel.daterangepicker', function (ev, picker) {
                    //do something, like clearing an input
                    $(this).val('');
                });
                return false;
            }

        }
    }
});

function EarlyStartAndDeliveryDateValidations() {

    var EStart = $("#txtestart").val();
    var dDate = $("#txtDelivery").val();

    if (Date.parse(dDate.split("/")[1] + "/" + dDate.split("/")[0] + "/" + dDate.split("/")[2].substr(0, 4)) > Date.parse(EStart.split("/")[1] + "/" + EStart.split("/")[0] + "/" + EStart.split("/")[2].substr(0, 4))) {
        jQuery('#modal-2').modal('show')
        $("#text").html("The part delivery date and time is after the appointment start date and time!!!")
        $('#txtDelivery').focus();
        return true;

    }
    return false;
}

function EarlyStartAndLateStartValidations() {
    var EStart = $("#txtestart").val();
    var LStart = $("#txtlstart").val();

    if ($('#hsrType').val() == "Corrective Repair" && $('#hCREntitlementFlag').val() == "Y" && $("#txtestart").val() == "" && $("#txtlstart").val() == "") {
        if ($("#taskduration").val() == 0) {
            jQuery('#modal-2').modal('show')
            $("#text").html("Visit Duration can't be zero. Please enter...")
            $('#taskduration').focus();
            return true;
        }
        return;
    }
    if (EStart != "" && LStart != "") {
        if (Date.parse(EStart.split("/")[1] + "/" + EStart.split("/")[0] + "/" + EStart.split("/")[2].substr(0, 4)) > Date.parse(LStart.split("/")[1] + "/" + LStart.split("/")[0] + "/" + LStart.split("/")[2].substr(0, 4))) {
            jQuery('#modal-2').modal('show')
            $("#text").html("Late Start Values Are In The Past.")
            $('#txtlstart').focus();
            return true;

        }
        if (Date.parse(EStart.split("/")[1] + "/" + EStart.split("/")[0] + "/" + EStart.split("/")[2].substr(0, 4)) == Date.parse(LStart.split("/")[1] + "/" + LStart.split("/")[0] + "/" + LStart.split("/")[2].substr(0, 4))) {

            var Etime = EStart.split(" ")[1];
            var Ltime = LStart.split(" ")[1];
            if (Ltime < Etime) {
                jQuery('#modal-2').modal('show')
                $("#text").html("Late Start Values Are In The Past.")
                $('#txtlstart').focus();
                return true;

            }
            if (Etime == Ltime) {
                jQuery('#modal-2').modal('show')
                $("#text").html("Early Start Should Be Different from Late Start")
                $('#txtlstart').focus();
                return true;
            }

        }
        // }



        //Current date validation with lLocal asset time
        var Currentdate = $("#tdefaultDate").val();
        if (Currentdate != "") {
            if (Date.parse(Currentdate.split("/")[1] + "/" + Currentdate.split("/")[0] + "/" + Currentdate.split("/")[2].substr(0, 4)) > Date.parse(EStart.split("/")[1] + "/" + EStart.split("/")[0] + "/" + EStart.split("/")[2].substr(0, 4))) {
                jQuery('#modal-2').modal('show');
                $("#text").html("Early Start Date Should Be Beyond Today's Date.");
                $('#txtestart').focus();
                return true;
            }
            if ($('#hsrType').val() != "Installation") {
                if (Date.parse(Currentdate.split("/")[1] + "/" + Currentdate.split("/")[0] + "/" + Currentdate.split("/")[2].substr(0, 4)) > Date.parse(LStart.split("/")[1] + "/" + LStart.split("/")[0] + "/" + LStart.split("/")[2].substr(0, 4))) {
                    jQuery('#modal-2').modal('show');
                    $("#text").html("Late Start should be more than Current Date and Time");
                    $('#txtlstart').focus();
                    return true;
                }
                if (Date.parse(Currentdate.split("/")[1] + "/" + Currentdate.split("/")[0] + "/" + Currentdate.split("/")[2].substr(0, 4)) == Date.parse(EStart.split("/")[1] + "/" + EStart.split("/")[0] + "/" + EStart.split("/")[2].substr(0, 4))) {

                    var Etime = EStart.split(" ")[1];
                    var Ctime = Currentdate.split(" ")[1];
                    if (Etime <= Ctime) {
                        jQuery('#modal-2').modal('show')
                        $("#text").html("Early Start Date Should Be Beyond Today's Date.")
                        $('#txtlstart').focus();
                        return true;

                    }
                }
                if (Math.ceil((new Date(LStart.split("/")[1] + "/" + LStart.split("/")[0] + "/" + LStart.split("/")[2].substr(0, 4)).getTime()) - (new Date(EStart.split("/")[1] + "/" + EStart.split("/")[0] + "/" + EStart.split("/")[2].substr(0, 4)).getTime())) / (1000 * 3600 * 24) > 30) {
                    jQuery('#modal-2').modal('show');
                    $("#text").html("Late Start and Early Start Difference Should not be greater than 30 Days !!!!");

                    $('#txtlstart').focus();
                    return true;
                }
            }
        }
    }


    if ($("#taskduration").val() == 0) {
        jQuery('#modal-2').modal('show')
        $("#text").html("Visit Duration can't be zero. Please enter...")
        $('#taskduration').focus();
        return true;
    }

    return false;
}

function taskDatetimeValidations() {

    var EStart = $("#txtestart").val();
    //Current date validation with lLocal asset time
    var today = $("#tdefaultDate").val();

    if (EStart != "" && today != "") {
        if (Date.parse(EStart.split("/")[1] + "/" + EStart.split("/")[0] + "/" + EStart.split("/")[2].substr(0, 4)) < Date.parse(today.split("/")[1] + "/" + today.split("/")[0] + "/" + today.split("/")[2].substr(0, 4))) {
            jQuery('#modal-2').modal('show')
            $("#text").html("Task Start date should be greater than or equals to current date time, Please select the valid Task Start Date")
            $('#txtestart').focus();
            return true;
        }
        if (Date.parse(EStart.split("/")[1] + "/" + EStart.split("/")[0] + "/" + EStart.split("/")[2].substr(0, 4)) == Date.parse(today.split("/")[1] + "/" + today.split("/")[0] + "/" + today.split("/")[2].substr(0, 4))) {
            var Etime1 = EStart.split(" ")[1];
            var Etime = Etime1.split(":").join("");
            var Ttoday1 = today.split(" ")[1];
            var Ttoday = Ttoday1.split(":").join("");
            if (Ttoday >= Etime) {
                jQuery('#modal-2').modal('show')
                $("#text").html("Task Start date should be greater than or equals to current date time, Please select the valid Task Start Date")
                $('#txtestart').focus();
                return true;

            }
        }
    }

    return false;
}

$("#cancltask").click(function (e) {
    //
    jQuery('#modal-1').modal('hide')
    e.preventDefault(); // <------------------ stop default behaviour of button
    var element = this;
    var dataToPost = {
        //  CancelReason: $('#IDstvCancelTaskValuesInSDT').val()
        CancelReason: $('#CanclTaskDropdown :selected').text()

    };
    $.ajax({
        url: "/RequestAppointmentBooking/CancelTask",
        type: "POST",
        data: dataToPost,
        success: function (data) {
            //
            if (data.message == "Cancelled") {
                jQuery('#modal-Reload').modal('show');
                $("#TextReload").html("Task Cancelled Successfully");
                $("#checkStatval").val("Cancelled");
                $("#btnrequestappointment").attr("disabled", "disabled");
                $("#btnrequestappointment").removeClass("btn btn-info");
                $("#btnrequestappointment").addClass("btn btn-dark");
                $("#modVis").attr("disabled", "disabled");
                $("#modVis").removeClass("btn btn-warning");
                $("#modVis").addClass("btn btn-dark");
                $("#ignore").attr("disabled", "disabled");
                $("#ignore").removeClass("btn btn-danger");
                $("#ignore").addClass("btn btn-dark");
                $("#btnCancelTask").attr("disabled", "disabled");
                $("#btnCancelTask").removeClass("btn btn-danger");
                $("#btnCancelTask").addClass("btn btn-dark");
                //<------------ submit form
            }
            else if (data.message == "Completed") {
                jQuery('#modal-Reload').modal('show');
                $("#TextReload").html("Already Completed Task Cannot be Cancelled.");
            }
            else {
                //jQuery('#modal-2').modal('show')
                //$("#text").html("Error occured during Task Cancellation (Exception" + data.message + ")")
                //US139 - Added by Ebaad - 19/4/2017 - User Friendly Error Message pop-up when error occurs while cancelling task
                $("#apptext").html("An error occurred while cancelling the Task !");
                $("#reqapp-moredetails").append("Exception: " + data.message);
                $("#reqapp-moredetails").hide();
                jQuery('#modal-moredetails').modal('show');

                $("#reqapp-moredetails").click(function () {
                    $("#reqapp-moredetails").toggle();
                    $("#applessdetailsDiv").show();
                    $("#appmoredetailsDiv").hide();
                });

                $("#reqapp-lessdetails").click(function () {
                    $("#reqapp-moredetails").toggle();
                    $("#appmoredetailsDiv").show();
                    $("#applessdetailsDiv").hide();
                });
            }
        },
        error: function () {
            //alert("An error has occured!!!");
            //US139 - Added by Ebaad - 19/4/2017 - User Friendly Error Message pop-up when error occurs while cancelling task
            $("#apptext").html("An error occurred while cancelling the Task !");
            $("#reqapp-moredetails").append("Exception: " + data.message);
            $("#reqapp-moredetails").hide();
            jQuery('#modal-moredetails').modal('show');

            $("#reqapp-moredetails").click(function () {
                $("#reqapp-moredetails").toggle();
                $("#applessdetailsDiv").show();
                $("#appmoredetailsDiv").hide();
            });

            $("#reqapp-lessdetails").click(function () {
                $("#reqapp-moredetails").toggle();
                $("#appmoredetailsDiv").show();
                $("#applessdetailsDiv").hide();
            });
        }
    })


})


$("#taskduration").keyup(function (evt) {


    var self = $(this);
    self.val(self.val().replace(/[^0-9]/g, ''));

    if (this.value.charAt(0) === '0')
        this.value = this.value.slice(1);

    if ((evt.which != 46 || self.val().indexOf('.') != -1) && (evt.which < 48 || evt.which > 57)) {
        evt.preventDefault();
    }


});
function PartCommentsChange() {
    PartDataModified = true;
    var deliverytype = $('#deliverytype :selected').text();

    var partComment = $("#txtPartComments").val();

    var deliveryDate = $("#txtDelivery").val();
    var address = $("#address").val();
    if (deliverytype != "" && deliverytype != "--Please Select--") {
        AddOrUpdateComment2(deliverytype, partComment, deliveryDate, address);

    }
    if (deliverytype != "" && deliverytype != "--Please Select--") {
        AddOrUpdateComment(deliverytype, partComment, deliveryDate, address);

    }
}
function DeliveryDateOnblur() {
    PartDataModified = true;

}
function DeliveryDateValidations() {
    var deliverytype = $('#deliverytype :selected').text();

    var partComment = $("#txtPartComments").val();

    var deliveryDate = $("#txtDelivery").val();
    var address = $("#address").val();
    if (deliverytype != "" && deliverytype != "--Please Select--") {
        AddOrUpdateComment2(deliverytype, partComment, deliveryDate, address);

    }
    if (deliverytype != "" && deliverytype != "--Please Select--") {
        AddOrUpdateComment(deliverytype, partComment, deliveryDate, address);

    }

    if ($('#hsrType').val() == "Corrective Repair" && $('#hCREntitlementFlag').val() == "Y" && $("#txtestart").val() == "") {

    }
    else {

        var EStart = $("#txtestart").val();
        var DeliDate = $("#txtDelivery").val();
        //Current date validation with Local asset time
        var Currentdate = $("#tdefaultDate").val();
        if (EStart == "") {
            jQuery('#modal-2').modal('show')
            $("#text").html("Please Enter Early Start Date.")
            $('#txtestart').focus();
            return true;
        }
        if (DeliDate != "" && Currentdate != undefined) {
            if (Date.parse(Currentdate.split("/")[1] + "/" + Currentdate.split("/")[0] + "/" + Currentdate.split("/")[2].substr(0, 4)) > Date.parse(DeliDate.split("/")[1] + "/" + DeliDate.split("/")[0] + "/" + DeliDate.split("/")[2].substr(0, 4))) {
                jQuery('#modal-2').modal('show');
                $("#text").html("The part delivery date should be greater than or equals to current date time, Please select the valid delivery date");
                $('#txtDelivery').focus();
                return true;
            }
            if (Date.parse(Currentdate.split("/")[1] + "/" + Currentdate.split("/")[0] + "/" + Currentdate.split("/")[2].substr(0, 4)) == Date.parse(DeliDate.split("/")[1] + "/" + DeliDate.split("/")[0] + "/" + DeliDate.split("/")[2].substr(0, 4))) {
                var Ctime = Currentdate.split(" ")[1];
                var Dtime = DeliDate.split(" ")[1];
                if (Ctime > Dtime) {
                    jQuery('#modal-2').modal('show');
                    $("#text").html("The part delivery date should be greater than or equals to current date time, Please select the valid delivery date");
                    $('#txtDelivery').focus();
                    return true;
                }
            }

        }


        if (EStart != "" && DeliDate != "") {
            if (Date.parse(EStart.split("/")[1] + "/" + EStart.split("/")[0] + "/" + EStart.split("/")[2].substr(0, 4)) < Date.parse(DeliDate.split("/")[1] + "/" + DeliDate.split("/")[0] + "/" + DeliDate.split("/")[2].substr(0, 4))) {
                jQuery('#modal-2').modal('show')
                if ($("#hsrType").val() == "Installation") {
                    $("#text").html("The part delivery date and time should not be more than Task Start Date and time,Please select the valid Delivery Date.");

                }
                else {
                    $("#text").html("The part delivery date and time should not be more than Early start date and time,Please select the valid Delivery Date.")

                }

                if (Date.parse(Currentdate.split("/")[1] + "/" + Currentdate.split("/")[0] + "/" + Currentdate.split("/")[2].substr(0, 4)) > Date.parse(EStart.split("/")[1] + "/" + EStart.split("/")[0] + "/" + EStart.split("/")[2].substr(0, 4))) {

                    if ($("#hsrType").val() == "Installation") {
                        $("#text").html("Task Start date should be greater than or equals to current date time, Please select the valid Task Start Date.");
                    }
                    else {
                        $("#text").html("Early Start Date should be greater than Current Date and Time,Please select a valid Early Start Date.");
                    }
                    $('#txtestart').focus();
                }
                if (Date.parse(Currentdate.split("/")[1] + "/" + Currentdate.split("/")[0] + "/" + Currentdate.split("/")[2].substr(0, 4)) == Date.parse(EStart.split("/")[1] + "/" + EStart.split("/")[0] + "/" + EStart.split("/")[2].substr(0, 4))) {
                    var Etime = EStart.split(" ")[1];
                    var Dtime = DeliDate.split(" ")[1];
                    var Ctime = Currentdate.split(" ")[1];
                    if (Ctime >= Etime) {

                        if ($("#hsrType").val() == "Installation") {
                            $("#text").html("Task Start date should be greater than or equals to current date time, Please select the valid Task Start Date.");

                        }
                        else {
                            $("#text").html("Early Start Date should be greater than Current Date and Time,Please select the valid Early Start Date.");

                        }
                        $('#txtestart').focus();
                    }
                    else {
                        $('#txtDelivery').focus();
                    }

                }
                return true;
            }
            if (Date.parse(EStart.split("/")[1] + "/" + EStart.split("/")[0] + "/" + EStart.split("/")[2].substr(0, 4)) == Date.parse(DeliDate.split("/")[1] + "/" + DeliDate.split("/")[0] + "/" + DeliDate.split("/")[2].substr(0, 4))) {
                var Etime = EStart.split(" ")[1];
                var Dtime = DeliDate.split(" ")[1];
                var Ctime = Currentdate.split(" ")[1];
                if (Dtime >= Etime) {
                    jQuery('#modal-2').modal('show')
                    if ($("#hsrType").val() == "Installation") {
                        $("#text").html("The part delivery date and time should not be more than or equal to Task Start Date and time,Please select a valid Delivery Date.");

                    }
                    else {
                        $("#text").html("The part delivery date and time should not be more than or equal to Early start date and time,Please select a valid Delivery Date.")
                    }
                    if (Date.parse(Currentdate.split("/")[1] + "/" + Currentdate.split("/")[0] + "/" + Currentdate.split("/")[2].substr(0, 4)) == Date.parse(EStart.split("/")[1] + "/" + EStart.split("/")[0] + "/" + EStart.split("/")[2].substr(0, 4))) {

                        if (Ctime >= Etime) {

                            if ($("#hsrType").val() == "Installation") {
                                $("#text").html("Task Start Date should be greater than Current Date and Time,Please select a valid Task Start Date.");

                            }
                            else {
                                $("#text").html("Early Start Date should be greater than Current Date and Time,Please select a valid Early Start Date.");

                            }
                            $('#txtestart').focus();
                        }
                        else {
                            $('#txtDelivery').focus();
                        }

                    }
                    return true;
                }

            }

        }
    }
}

function ValidateSSOid(element) {
    if ($('.editOption').val() != '') {
        var ssoid = {
            SSOID: element.value

        };
        $.ajax({
            url: '../Home/GetValidFSESSOID',
            type: "POST",
            data: ssoid,
            success: function (data) {
                if (data == '') {
                    SSOIDinvalid = false;
                    $("#imgPreferedPSEValid").hide();
                    $("#imgPreferedPSECross").show();
                }
                else {
                    $('.editOption').attr('title', data);
                    SSOIDinvalid = true;
                    $("#imgPreferedPSEValid").show();
                    $("#imgPreferedPSECross").hide();
                }
            }
        })
    }
    else {
        $("#imgPreferedPSEValid").hide();
        $("#imgPreferedPSECross").hide();
    }
}
var GetSiteSystemData, arrSite, arrSystem = "";
function GetSiteSytemClickTaskCount() {

    $.ajax({
        type: "POST",
        url: "/Home/GetSiteSytemClickTaskCount",
    }).done(function (data) {
        GetSiteSystemData = data;
        $('#btnActualDependencySite')[0].innerHTML = data.SiteCountActual;
        $('#btnPotentialDependencySite')[0].innerHTML = data.SiteCountPotential;

        $('#btnActualDependencySystem')[0].innerHTML = data.SystemCountActual;
        $('#btnPotentialDependencySystem')[0].innerHTML = data.SystemCountPotential;
        clcikTimeSpan = "Click Response(Sec): " + data.receiveTime;
        $("#crepTime").html(clcikTimeSpan)
    });




}

function CloseWindow() {
    window.close();
}


var options = [];

$('.dropdown-menu a').on('click', function (event) {

    var $target = $(event.currentTarget),
      val = $target.attr('data-value'),
      $inp = $target.find('input'),
      idx;

    if ((idx = options.indexOf(val)) > -1) {
        options.splice(idx, 1);
        setTimeout(function () {
            $inp.prop('checked', false)
        }, 0);
    } else {
        options.push(val);
        setTimeout(function () {
            $inp.prop('checked', true)
        }, 0);
    }

    $(event.target).blur();

    console.log(options);
    return false;
});

$('#txtestart').daterangepicker({
    "singleDatePicker": true,
    "showDropdowns": true,
    "timePicker": true,
    "use12hours": true,
    "autoApply": true,
    "locale": {
        "format": "DD/MM/YYYY HH:mm ",
        "separator": " - ",
        "applyLabel": "Apply",
        "cancelLabel": "Cancel",
        "fromLabel": "From",
        "toLabel": "To",
        "customRangeLabel": "Custom",
        "daysOfWeek": [
        "Su",
        "Mo",
        "Tu",
        "We",
        "Th",
        "Fr",
        "Sa"
        ],
        "monthNames": [
        "January",
        "February",
        "March",
        "April",
        "May",
        "June",
        "July",
        "August",
        "September",
        "October",
        "November",
        "December"
        ],
        "firstDay": 1
    },
}, function (start, end, label) {
    console.log("New date range selected: ' + start.format('YYYY-MM-DD') + ' to ' + end.format('YYYY-MM-DD') + ' (predefined range: ' + label + ')");
});






$(function () {

    $("#range_27").ionRangeSlider({

        type: "double",
        min: 1000000,
        max: 2000000,
        grid: true,
        force_edges: true
    });
    $("#range").ionRangeSlider({
        hide_min_max: true,
        keyboard: true,
        min: 0,
        max: 5000,
        from: 1000,
        to: 4000,
        type: 'double',
        step: 1,
        prefix: "$",
        grid: true
    });
    $("#range_25").ionRangeSlider({
        type: "double",
        min: 1000000,
        max: 2000000,
        grid: true
    });
    $("#range_26").ionRangeSlider({
        type: "double",
        min: 0,
        max: 10000,
        step: 500,
        grid: true,
        grid_snap: true
    });
    $("#range_31").ionRangeSlider({
        type: "double",
        min: 0,
        max: 100,
        from: 30,
        to: 70,
        from_fixed: true
    });
    $(".range_min_max").ionRangeSlider({
        type: "double",
        min: 0,
        max: 100,
        from: 30,
        to: 70,
        max_interval: 50
    });
    $(".range_time24").ionRangeSlider({
        min: +moment().subtract(12, "hours").format("X"),
        max: +moment().format("X"),
        from: +moment().subtract(6, "hours").format("X"),
        grid: true,
        force_edges: true,
        prettify: function (num) {
            var m = moment(num, "X");
            return m.format("Do MMMM, HH:mm");
        }
    });
});

$(function ($) {

    $(".knob").knob({
        change: function (value) {
        },
        release: function (value) {
            console.log("release : " + value);
        },
        cancel: function () {
            console.log("cancel : ", this);
        },
        /*format : function (value) {
         return value + '%';
         },*/
        draw: function () {

            // "tron" case
            if (this.$.data('skin') == 'tron') {

                this.cursorExt = 0.3;

                var a = this.arc(this.cv) // Arc
                    ,
                    pa // Previous arc
                    , r = 1;

                this.g.lineWidth = this.lineWidth;

                if (this.o.displayPrevious) {
                    pa = this.arc(this.v);
                    this.g.beginPath();
                    this.g.strokeStyle = this.pColor;
                    this.g.arc(this.xy, this.xy, this.radius - this.lineWidth, pa.s, pa.e, pa.d);
                    this.g.stroke();
                }

                this.g.beginPath();
                this.g.strokeStyle = r ? this.o.fgColor : this.fgColor;
                this.g.arc(this.xy, this.xy, this.radius - this.lineWidth, a.s, a.e, a.d);
                this.g.stroke();

                this.g.lineWidth = 2;
                this.g.beginPath();
                this.g.strokeStyle = this.o.fgColor;
                this.g.arc(this.xy, this.xy, this.radius - this.lineWidth + 1 + this.lineWidth * 2 / 3, 0, 2 * Math.PI, false);
                this.g.stroke();

                return false;
            }
        }
    });

    // Example of infinite knob, iPod click wheel
    var v, up = 0,
        down = 0,
        i = 0,
        $idir = $("div.idir"),
        $ival = $("div.ival"),
        incr = function () {
            i++;
            $idir.show().html("+").fadeOut();
            $ival.html(i);
        },
        decr = function () {
            i--;
            $idir.show().html("-").fadeOut();
            $ival.html(i);
        };
    $("input.infinite").knob({
        min: 0,
        max: 20,
        stopper: false,
        change: function () {
            if (v > this.cv) {
                if (up) {
                    decr();
                    up = 0;
                } else {
                    up = 1;
                    down = 0;
                }
            } else {
                if (v < this.cv) {
                    if (down) {
                        incr();
                        down = 0;
                    } else {
                        down = 1;
                        up = 0;
                    }
                }
            }
            v = this.cv;
        }
    });
});



