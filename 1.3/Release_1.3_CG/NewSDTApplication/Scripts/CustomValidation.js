/// <reference path="jquery-2.1.4.js" />
var gduration;
var gES;
var gLS;
var gTaskNotes;
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
$(document).ready(function () {

    if ($("#ValidateSystemId").val() == "SystemIDNotExit") {
        var message = "System ID does not exist in click.";
        $("#siteidtext").html(message);
        jQuery('#sitemodel').modal('show');

    }
    if ($("#ValidateShipToSite").val() == "SiteIDNotExit") {
        var message = "Site ID does not exist in click.";
        $("#siteidtext").html(message);
        jQuery('#sitemodel').modal('show');

    }

    if (($("#ValidateSystemId").val() == "SystemIDNotExit") && ($("#ValidateShipToSite").val() == "SiteIDNotExit")) {
        var message = "System ID/Site ID does not exist in click.";
        $("#siteidtext").html(message);
        jQuery('#sitemodel').modal('show');
    }

    //alert("In Custom Validation");
    //alert($("#hsrType").val());
    $("#txtDelivery").val('');

    //if ($('#hsrType').val() == "Corrective Repair") {
    //    $('#txtlstart').attr("disabled", "disabled");
    //    $('#txtestart').attr("disabled", "disabled");
    //}
    //else {
    //    $('#txtlstart').removeAttr('disabled');
    //    $('#txtestart').removeAttr('disabled');
    //}
    //var date1 = new Date("7/Nov/2012 20:30:00");
    //var date2 = new Date("20/Nov/2012 19:15:00");
    //siebleTimeSpan = new Date($("#hsiebleReceiveTime").val()).getTime() - new Date($("#hseibleSendTime").val()).getTime();
    siebleTimeSpan = "Siebel Response(Sec): " + $("#hsiebleReceiveTime").val();
    $("#srepTime").html(siebleTimeSpan);

    // clcikTimeSpan = new Date($("#hclickReceiveTime").val()).getTime() - new Date($("#hclickSendTime").val()).getTime();
    //// clcikTimeSpan = totalTimeSpan - siebleTimeSpan;

    //// clcikTimeSpan = "Click Response Time in Sec: " + clcikTimeSpan / 1000;


    //$("#crepTime").html(clcikTimeSpan);
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
        //gLS = $("#txtlstart").val();
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
        //var total = (parseInt(taskduration)) + (parseInt(taskhrs));

        //var htmlhrs ="= " + total + " Hour(s)";
        //$("#totalhrs").html(htmlhrs);
    }
    if ($("#taskduration").val() == "" || $("#taskduration").val() == null) {
        gduration = "0";
    }
    else {
        gduration = $("#taskduration").val();
    }

    gTaskNotes = $("#tasknotes").val();

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
                //$(this).find('FseData').each(function () {

                //    var OptionText = $(this).find('optionText').text();
                //    var OptionValue = $(this).find('optionValue').text();

                //    var option = $("<option>" + OptionText + "</option>");
                //    option.attr("value", OptionValue);

                //    select1.append(option);

                //});

                //$(this).find('FseData').each(function () {

                //    var OptionText = $(this).find('optionText').text();
                //    var OptionValue = $(this).find('optionValue').text();

                //    var option = $("<option>" + OptionText + "</option>");
                //    option.attr("value", OptionValue);

                //    select2.append(option);
                //});
            });


            if (fse1data != "") {
                $('#Skillfse1 option:contains("' + fse1data + '")').attr('selected', 'selected');
                //$('#Skillfse1').val(2);

            }
            else {
                $('#Skillfse1').val(-1);
            }

            if (fse2data != "") {
                $('#Skillfse2 option:contains("' + fse2data + '")').attr('selected', 'selected');
                //$('#Skillfse1').val(0);
            }
            else {
                $('#Skillfse2').val(-1);
            }

            if (fse3data != "") {
                $('#Skillfse3 option:contains("' + fse3data + '")').attr('selected', 'selected');
                //$('#Skillfse1').val(2);
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
    //if ($("#hsrType").val() != "Installation") {

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
    //}


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
        // $("#imgPrevious").removeAttr("onclick");
        //$("#imgnext").removeAttr("onclick");
        $("#imgdel").removeAttr("onclick");

        $("#btnSite").attr("disabled", "disabled");
        $("#btnSystem").attr("disabled", "disabled");
        $("#btnCancelTask").attr("disabled", "disabled");
        $("#btnCancelTask").removeClass("btn btn-danger");
        $("#btnCancelTask").addClass("btn btn-dark");
    }

    if ($('#checkStatval').val() == "Assigned" || $('#checkStatval').val() == "Acknowledged" || $('#checkStatval').val() == "En Route" || $('#checkStatval').val() == "On Site" || $('#checkStatval').val() == "Completed" || $('#checkStatval').val() == "Incomplete" || $('#checkStatval').val() == "Cancelled" || $('#checkStatval').val() == "Rejected") {
        //alert("in checkStatval ");
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
    //$('#SDTrequestAppointmentStartDte').val($('#happointmentStart').val());
    //$('#SDTrequestAppointmentFinishDte').val($('#happointmentFinish').val());


    //var SDTrequestAppointmentStartDte = $('#SDTrequestAppointmentStartDte')[0].placeholder;
    //var SDTrequestAppointmentFinishDte = $('#SDTrequestAppointmentFinishDte')[0].placeholder;

    //if (SDTrequestAppointmentStartDte == "1/1/0001")
    //    $('#APPSTART').hide();
    //// $('#SDTrequestAppointmentStartDte').val('not available');

    //if (SDTrequestAppointmentFinishDte == "1/1/0001")
    //    $('#APPFINISH').hide();
    ////$('#SDTrequestAppointmentFinishDte').val('not available');

    //function disableBack() { window.history.forward() }
    //window.onload = disableBack();
    //window.onpageshow = function (evt) { if (evt.persisted) disableBack() }
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



    // $("#SSOI1").combobox();
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
        //  var selected = $('option:selected', this).attr('class');

        //  var optionText = $('.editOption').val();

        //if (selected == "editable") {
        //    $('.editOption').css("visibility", "visible");
        //    $('.editOption').show();


        //} else {
        //    $('.editOption').hide();
        //}

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
                //$('#chkfse1').attr('checked', true);

            }
            //var str = $("#hdnReqPrefFlag").val();
            ////$(".selectpicker option[value='305026031']").addClass("glyphicon glyphicon-ok check-mark");
            //if ($("#hdnReqPrefFlag").val() == "R") {
            //    //$("#chkfse1").prop("checked", true);
            //    $(".icheckbox_flat-green").addClass("checked");
            //} else {
            //    //$("#chkfse1").prop("checked", false);
            //    $(".icheckbox_flat-green").removeClass("checked");
            //}

            if ($("#hsrType").val() == "Installation") {
                var str = $("#hdnReqPrefFlag").val();
                //$(".selectpicker option[value='305026031']").addClass("glyphicon glyphicon-ok check-mark");
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
                //$(".selectpicker option[value='305026031']").addClass("glyphicon glyphicon-ok check-mark");
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

            //var url = $(this).children(":selected").data("url");
            //var val = $(".selectpicker").children[data-original-index].getAttribute("value");

            //HTMLSelectElement.children[data-original-index].getAttribute("value")
            // <li data-original-index="1">
            // <a tabindex="0" class="glyphicon glyphicon-ok check-mark" data-tokens="null">
            // <span class="text">305026031 || CHUNG MIN LEE || SDT Schedule</span>
            //<span class="glyphicon glyphicon-ok check-mark"></span></a></li>


            //<li class="selected" data-original-index="0" >
            //    <a tabindex="0" data-tokens="null">
            //        <span class="text">305000082 || George Qiu || Siebel </span>
            //<span class="glyphicon glyphicon-ok check-mark"></span></a></li>
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

        //alert("request appointment");
        //
        e.preventDefault();
        var SSOID1;
        var SSOID2;
        var SSOID3;
        var ID1;
        var ID2;
        var ID3;
        var fseSkill;
        var isrequiredfse;
        //var x = $("#checkbox").is(":checked");
        isrequiredfse = $("#chkfse1").is(":checked");
        //alert($("#Skillfse1").val());
        //  fseSkill = $("#Skillfse1").val() + "," + $("#Skillfse2").val() + "," + $("#Skillfse3").val();
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
            //alert(values);
        }
        else {
            ID1 = null;
        }
        var select = $("#SSoid3").val();
        if (select != null) {
            SSOID3 = select.toString().split('||');
            ID3 = SSOID3[0]
            //alert(select);
        }
        else {
            ID3 = null;
        }
        var val = $("#SSoid2").val();
        if (val != null) {
            SSOID2 = val.toString().split('||');
            ID2 = SSOID2[0]
            //alert(val);
        }
        else {
            ID2 = null;
        }
        //alert("Before Condition");
        //if ($("#hsrType").val() != "Installation") {
        //    //alert("Inside ES and LS");
        //    var EStart = $("#txtestart").val();
        //    //var EstartDate = $("#txtestart").val();

        //    var LStart = $("#txtlstart").val();
        //    var AppWindow = $('#appWindow input:radio:checked');
        //    var IsValidationFail = EarlyStartAndLateStartValidations();
        //    if (IsValidationFail)
        //        return true;
        //}
        //else {
        //    var IsInstallValidationsFail = taskDatetimeValidations()
        //    if (IsInstallValidationsFail)
        //        return true;
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
        //var IsDeliveryDateValidationFail = DeliveryDateValidations()
        //if (IsDeliveryDateValidationFail)
        //    return true;
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
                    //if (addr2[i][j][4] == "KOREA, REPUBLIC OF") {

                    //    addr2[i][j][4] = "KOREA REPUBLIC OF";
                    //}
                    if (addr2[i][j][4] == "South Korea") {

                        addr2[i][j][4] = "KOREA, REPUBLIC OF";
                    }
                    //Before Modify Check whether Customer site selected or not
                    //if (addr2[i][j][0] == "Customer Site") {
                    //    flg=true
                    //}
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
                    //if (addr1[i][j][4] == "KOREA, REPUBLIC OF") {

                    //    addr1[i][j][4] = "KOREA REPUBLIC OF";
                    //}
                    if (addr1[i][j][4] == "South Korea") {

                        addr1[i][j][4] = "KOREA, REPUBLIC OF";
                    }
                    //Before Create Check whether Customer site selected or not
                    //if (addr1[i][j][0] == "Customer Site") {
                    //    flg = true
                    //}
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


        //}
        s += ",";
        if (s == ",") {
            s = "";
        }
        //
        //alert(s);
        var tasknotes = $('#tasknotes').val();
        if (tasknotes != "") {

            // tasknotes = tasknotes.replace(/\n/g, "\\n");

            if (tasknotes.indexOf(',') != -1) {
                tasknotes = tasknotes.replace(/,/g, "");

            }
        }

        var parComments = $('#txtPartComments').val();

        if (parComments != "") {
            if (parComments.indexOf(',') != -1) {
                parComments = parComments.replace(/,/g, "");
            }
        }
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
                IsRequiredfse: isrequiredfse
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
                IsRequiredfse: isrequiredfse
            };
        }

        // if (dtLateStart > dtEarlyStart) {
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
        //  }
        //else {
        //    jQuery('#modal-2').modal('show')
        //    $("#text").html("Late Start should be greater than Early Start!!!!")
        //    $('#txtlstart').focus();
        //    return true;
        //}

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
    //alert($("#htDurationInstall").val());
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
    //alert(total);

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
    // alert("sdfdfd");
    window.close();
})

$('#btnPotentialDependencySite').click(function () {
    //var IsValidationFail = EarlyStartAndLateStartValidations();
    // alert($("#txtestart").val() + " " + $("#txtlstart").val() + " " + $("#taskduration").val().toString());
    //if (IsValidationFail)
    //    return true;
    // e.preventDefault();
    //alert($('#hTaskSiteID').val())
    //added by barnali for part pick up 



    var SSOID1;
    var SSOID2;
    var SSOID3;
    var ID1;
    var ID2;
    var ID3;
    var fseSkill;
    var isrequiredfse;
    //var x = $("#checkbox").is(":checked");
    isrequiredfse = $("#chkfse1").is(":checked");
    //alert($("#Skillfse1").val());
    //Comment by Phani Kanth because of skill 2 & skill 3 are disabled
    //fseSkill = $("#Skillfse1").val() + "," + $("#Skillfse2").val() + "," + $("#Skillfse3").val();
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
        //alert(values);
    }
    else {
        ID1 = null;
    }
    var select = $("#SSoid3").val();
    if (select != null) {
        SSOID3 = select.toString().split('||');
        ID3 = SSOID3[0]
        //alert(select);
    }
    else {
        ID3 = null;
    }
    var val = $("#SSoid2").val();
    if (val != null) {
        SSOID2 = val.toString().split('||');
        ID2 = SSOID2[0]
        //alert(val);
    }
    else {
        ID2 = null;
    }
    //alert("Before Condition");
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
    //var IsDeliveryDateValidationFail = DeliveryDateValidations()
    //if (IsDeliveryDateValidationFail)
    //    return true;
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

    //

    for (i = 0; i < addr2.length; i++) {

        for (j = 0; j < addr2[i].length; j++) {
            //No need to chnage change the Name
            //if (addr2[i][j][4] == "KOREA, REPUBLIC OF") {

            //    addr2[i][j][4] = "KOREA REPUBLIC OF";
            //}
            if (addr2[i][j][4] == "South Korea") {

                addr2[i][j][4] = "KOREA, REPUBLIC OF";
            }
            //Before Modify Check whether Customer site selected or not
            //if (addr2[i][j][0] == "Customer Site") {
            //    flg=true
            //}
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

    //}
    s += ",";
    if (s == ",") {
        s = "";
    }
    //alert(s);
    var tasknotes = $('#tasknotes').val();

    if (tasknotes != "") {
        //tasknotes = tasknotes.replace(/\n/g, "\\n");

        if (tasknotes.indexOf(',') != -1) {
            tasknotes = tasknotes.replace(/,/g, "");
        }
    }

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
    //var x = $("#checkbox").is(":checked");
    isrequiredfse = $("#chkfse1").is(":checked");
    //alert($("#Skillfse1").val());
    //fseSkill = $("#Skillfse1").val() + "," + $("#Skillfse2").val() + "," + $("#Skillfse3").val();
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
        //alert(values);
    }
    else {
        ID1 = null;
    }
    var select = $("#SSoid3").val();
    if (select != null) {
        SSOID3 = select.toString().split('||');
        ID3 = SSOID3[0]
        //alert(select);
    }
    else {
        ID3 = null;
    }
    var val = $("#SSoid2").val();
    if (val != null) {
        SSOID2 = val.toString().split('||');
        ID2 = SSOID2[0]
        //alert(val);
    }
    else {
        ID2 = null;
    }
    //alert("Before Condition");
    var isDisabled = $('#txtDelivery').prop('disabled');

    if ($('#hsrType').val() == "Installation") {
        // var IsInstallValidationsFail = taskDatetimeValidations()
        // if (IsInstallValidationsFail)
        //   return true;
    }
    else {
        var AppWindow = $('#appWindow input:radio:checked');

        //if ($('#hsrType').val() == "Corrective Repair" && $('#hCREntitlementFlag').val() == "Y" && $("#txtestart").val() == "" || $("#txtlstart").val() == "") {


        //}
        //else {
        //  //  var IsValidationFail = EarlyStartAndLateStartValidations();
        //  //  if (IsValidationFail)
        //      //  return true;
        //}
    }
    //var IsDeliveryDateValidationFail = DeliveryDateValidations()
    //if (IsDeliveryDateValidationFail)
    //    return true;
    //if ($('#hsrType').val() == "Corrective Repair" && $('#hCREntitlementFlag').val() == "Y" && $("#txtestart").val() == "") {

    //}
    //else {
    //    if (!isDisabled) {
    //        var IsDeliveryDateValidationFail = DeliveryDateValidations()

    //        if (IsDeliveryDateValidationFail)
    //            return true;
    //    }
    //}

    //if ($('#hsrType').val() == "Installation") {
    //    if ($('.editOption').val() != "") {
    //        if (SSOIDinvalid == false) {
    //            jQuery('#modal-2').modal('show')
    //            $("#text").html("SSOID is Invalid. Please enter a valid SSOID");
    //            return;
    //        }
    //    }
    //}


    //if ($("#taskduration").val() == 0 || $("#taskduration").val() == null) {
    //    jQuery('#modal-2').modal('show')
    //    if ($('#hsrType').val() != "Installation")
    //    { $("#text").html("Visit Duration can't be zero. Please enter...") }
    //    else { $("#text").html("Task Duration can't be zero. Please enter...") }
    //    $('#taskduration').focus();
    //    return true;
    //}

    //if ($('#hsrType').val() == "Installation") {
    //    if ($("#htDurationInstall").val() == 0 || $("#htDurationInstall").val() == null) {
    //        $("#text").html("Task Duration can't be zero. Please enter...");
    //        jQuery('#modal-2').modal('show')
    //        $('#taskduration').focus();
    //        return true;
    //    }
    //}


    //if ($('#hsrType').val() != "Installation") {
    //    if ($("#taskduration").val() == 0 || $("#taskduration").val() == null) {
    //        jQuery('#modal-2').modal('show')

    //        $("#text").html("Visit Duration can't be zero. Please enter...")
    //        $('#taskduration').focus();
    //        return true;
    //    }

    //}
    //if (GetSiteSystemData.SiteCountActual <= 0) {
    //    jQuery('#modal-2').modal('show')
    //    $("#text").html("No Actual Site Dependency Job Found.")
    //    return true;
    //}


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
            //if (addr2[i][j][4] == "KOREA, REPUBLIC OF") {

            //    addr2[i][j][4] = "KOREA REPUBLIC OF";
            //}
            if (addr2[i][j][4] == "South Korea") {

                addr2[i][j][4] = "KOREA, REPUBLIC OF";
            }
            //Before Modify Check whether Customer site selected or not
            //if (addr2[i][j][0] == "Customer Site") {
            //    flg=true
            //}
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

    //}
    s += ",";
    if (s == ",") {
        s = "";
    }
    //alert(s);
    var tasknotes = $('#tasknotes').val();
    if (tasknotes != "") {
        // tasknotes = tasknotes.replace(/\n/g, "\\n");

        if (tasknotes.indexOf(',') != -1) {
            tasknotes = tasknotes.replace(/,/g, "");
        }
    }

    // Changes done by Raju, send array to controller intead of string

    var installDuration = ($("#htDurationInstall").val() * 60).toString();
    if ($("#hsrType").val() == "Installation") {
        var dataToPost = {

            TaskSiteID: $('#hTaskSiteID').val(),
            EStart: $("#txtestart").val(),
            LStart: "",
            Duration: installDuration,
            TaskNotes: tasknotes,
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

//$("#taskduration").keyup(function () {
//    var taskduration = $("#taskduration").val();
//    if (taskduration < 1) {
//        $("#taskduration").val();
//    }
//});

$('#btnPotentialDependencySystem').click(function () {
    //var IsValidationFail = EarlyStartAndLateStartValidations();

    //if (IsValidationFail)
    //    return true;
    // alert($('#hTaskSystemID').val());

    //added by barnali for part pick up 

    var SSOID1;
    var SSOID2;
    var SSOID3;
    var ID1;
    var ID2;
    var ID3;
    var fseSkill;
    var isrequiredfse;
    //var x = $("#checkbox").is(":checked");
    isrequiredfse = $("#chkfse1").is(":checked");
    //alert($("#Skillfse1").val());
    // fseSkill = $("#Skillfse1").val() + "," + $("#Skillfse2").val() + "," + $("#Skillfse3").val();
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
        //alert(values);
    }
    else {
        ID1 = null;
    }
    var select = $("#SSoid3").val();
    if (select != null) {
        SSOID3 = select.toString().split('||');
        ID3 = SSOID3[0]
        //alert(select);
    }
    else {
        ID3 = null;
    }
    var val = $("#SSoid2").val();
    if (val != null) {
        SSOID2 = val.toString().split('||');
        ID2 = SSOID2[0]
        //alert(val);
    }
    else {
        ID2 = null;
    }
    //alert("Before Condition");
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
    //var IsDeliveryDateValidationFail = DeliveryDateValidations()
    //if (IsDeliveryDateValidationFail)
    //    return true;
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
    //if ($("#taskduration").val() == 0 || $("#taskduration").val() == null) {
    //    jQuery('#modal-2').modal('show')
    //    if ($('#hsrType').val() != "Installation")
    //    { $("#text").html("Visit Duration can't be zero. Please enter...") }
    //    else { $("#text").html("Task Duration can't be zero. Please enter...") }
    //    $('#taskduration').focus();
    //    return true;
    //}


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
            //if (addr2[i][j][4] == "KOREA, REPUBLIC OF") {

            //    addr2[i][j][4] = "KOREA REPUBLIC OF";
            //}
            if (addr2[i][j][4] == "South Korea") {

                addr2[i][j][4] = "KOREA, REPUBLIC OF";
            }
            //Before Modify Check whether Customer site selected or not
            //if (addr2[i][j][0] == "Customer Site") {
            //    flg=true
            //}
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
    //}
    s += ",";
    if (s == ",") {
        s = "";
    }
    //alert(s);
    var tasknotes = $('#tasknotes').val();
    if (tasknotes != "") {
        //tasknotes = tasknotes.replace(/\n/g, "\\n");

        if (tasknotes.indexOf(',') != -1) {
            tasknotes = tasknotes.replace(/,/g, "");
        }
    }
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
    //var x = $("#checkbox").is(":checked");
    isrequiredfse = $("#chkfse1").is(":checked");
    //alert($("#Skillfse1").val());
    //  fseSkill = $("#Skillfse1").val() + "," + $("#Skillfse2").val() + "," + $("#Skillfse3").val();
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
        //alert(values);
    }
    else {
        ID1 = null;
    }
    var select = $("#SSoid3").val();
    if (select != null) {
        SSOID3 = select.toString().split('||');
        ID3 = SSOID3[0]
        //alert(select);
    }
    else {
        ID3 = null;
    }
    var val = $("#SSoid2").val();
    if (val != null) {
        SSOID2 = val.toString().split('||');
        ID2 = SSOID2[0]
        //alert(val);
    }
    else {
        ID2 = null;
    }
    //alert("Before Condition");
    var isDisabled = $('#txtDelivery').prop('disabled');

    if ($('#hsrType').val() == "Installation") {
        //   var IsInstallValidationsFail = taskDatetimeValidations()
        //   if (IsInstallValidationsFail)
        //    return true;
    }
    else {
        var AppWindow = $('#appWindow input:radio:checked');

        //if ($('#hsrType').val() == "Corrective Repair" && $('#hCREntitlementFlag').val() == "Y" && $("#txtestart").val() == "" || $("#txtlstart").val() == "") {


        //}
        //else {
        //    var IsValidationFail = EarlyStartAndLateStartValidations();
        //    if (IsValidationFail)
        //        return true;
        //}
    }
    //var IsDeliveryDateValidationFail = DeliveryDateValidations()
    //if (IsDeliveryDateValidationFail)
    //    return true;
    //if ($('#hsrType').val() == "Corrective Repair" && $('#hCREntitlementFlag').val() == "Y" && $("#txtestart").val() == "") {

    //}
    //else {
    //    if (!isDisabled) {
    //        var IsDeliveryDateValidationFail = DeliveryDateValidations()

    //        if (IsDeliveryDateValidationFail)
    //            return true;
    //    }
    //}

    //if ($('#hsrType').val() == "Installation") {
    //    if ($('.editOption').val() != "") {
    //        if (SSOIDinvalid == false) {
    //            jQuery('#modal-2').modal('show')
    //            $("#text").html("SSOID is Invalid. Please enter a valid SSOID");
    //            return;
    //        }
    //    }
    //}


    //if ($('#hsrType').val() == "Installation") {
    //    if ($("#htDurationInstall").val() == 0 || $("#htDurationInstall").val() == null) {
    //        $("#text").html("Task Duration can't be zero. Please enter...");
    //        jQuery('#modal-2').modal('show')
    //        $('#taskduration').focus();
    //        return true;
    //    }
    //}


    //if ($('#hsrType').val() != "Installation") {
    //    if ($("#taskduration").val() == 0 || $("#taskduration").val() == null) {
    //        jQuery('#modal-2').modal('show')

    //        $("#text").html("Visit Duration can't be zero. Please enter...")
    //        $('#taskduration').focus();
    //        return true;
    //    }

    //}


    //if (GetSiteSystemData.SystemCountActual <= 0) {
    //    jQuery('#modal-2').modal('show')
    //    $("#text").html("No Actual System Dependency Job Found.")
    //    return true;
    //}

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
            //if (addr2[i][j][4] == "KOREA, REPUBLIC OF") {

            //    addr2[i][j][4] = "KOREA REPUBLIC OF";
            //}
            if (addr2[i][j][4] == "South Korea") {

                addr2[i][j][4] = "KOREA, REPUBLIC OF";
            }
            //Before Modify Check whether Customer site selected or not
            //if (addr2[i][j][0] == "Customer Site") {
            //    flg=true
            //}
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

    //}
    s += ",";
    if (s == ",") {
        s = "";
    }
    //alert(s);
    var tasknotes = $('#tasknotes').val();
    if (tasknotes != "") {
        // tasknotes = tasknotes.replace(/\n/g, "\\n");

        if (tasknotes.indexOf(',') != -1) {
            tasknotes = tasknotes.replace(/,/g, "");
        }
    }
    // Changes done by Raju, send array to controller intead of string

    var installDuration = ($("#htDurationInstall").val() * 60).toString();

    if ($("#hsrType").val() == "Installation") {
        var dataToPost = {
            TaskSystemID: $('#hTaskSystemID').val(),
            //TaskSiteID: $('#hTaskSiteID').val(),
            EStart: $("#txtestart").val(),
            LStart: "",
            Duration: installDuration,
            //Profile: AppWindow.attr('value'),
            TaskNotes: tasknotes,
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
            //TaskSiteID: $('#hTaskSiteID').val(),
            EStart: $("#txtestart").val(),
            LStart: $("#txtlstart").val(),
            Duration: $("#taskduration").val().toString(),
            //Profile: AppWindow.attr('value'),
            TaskNotes: tasknotes,
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
        //if ($("#Skillfse1").val() != 0) {
        //    fseskills1 = "Skill Level : " + "Level " + $("#Skillfse1").val();
        //} else {
        //    fseskills1 = "Skill Level : " + "Any FE";
        //}
        fseskills1 = "Skill Level : " + $("#Skillfse1 option:selected").html();
        //} else {
        if ($("#Skillfse1 option:selected").html() == 'None') {
            fseskills1 = "";
        }
    } else {
        fseskills1 = "";
    }


    var tNotes = "Task Notes : " + $("#tasknotes").val();
    var ssoid;
    if (myselect != null)
    { ssoid = "SSOID : " + myselect; }
    else {
        ssoid = "";
    }

    var alertMessage = "Do You Really Want To Modify This Task<br>";

    if (gduration == $("#taskduration").val() && gES != $("#txtestart").val() && gLS != $("#txtlstart").val()) {
        jQuery('#modal-3').modal('show');

        if (gTaskNotes == $("#tasknotes").val())
            $("#modifypop").html(alertMessage + sart + "<br>" + late + "<br>" + ssoid + "</br>" + fseskills1);
        else
            $("#modifypop").html(alertMessage + sart + "<br>" + late + "<br>" + tNotes + "<br>" + ssoid + "</br>" + fseskills1);
    }
    else if (gduration != $("#taskduration").val() && gES != $("#txtestart").val() && gLS == $("#txtlstart").val()) {
        jQuery('#modal-3').modal('show');
        if (gTaskNotes == $("#tasknotes").val())
            $("#modifypop").html(alertMessage + task + "</br>" + sart + "<br>" + ssoid + "</br>" + fseskills1);
        else
            $("#modifypop").html(alertMessage + task + "</br>" + sart + "</br>" + tNotes + "<br>" + ssoid + "</br>" + fseskills1);
    }
    else if (gduration != $("#taskduration").val() && gES == $("#txtestart").val() && gLS != $("#txtlstart").val()) {
        jQuery('#modal-3').modal('show');
        if (gTaskNotes == $("#tasknotes").val())
            $("#modifypop").html(alertMessage + task + "</br>" + late + "<br>" + ssoid + "</br>" + fseskills1);
        else
            $("#modifypop").html(alertMessage + task + "</br>" + late + "</br>" + tNotes + "<br>" + ssoid + "</br>" + fseskills1);
    }
    else if (gduration == $("#taskduration").val() && gES == $("#txtestart").val() && gLS != $("#txtlstart").val()) {
        jQuery('#modal-3').modal('show');
        if (gTaskNotes == $("#tasknotes").val())
            $("#modifypop").html(alertMessage + late + "<br>" + ssoid + "</br>" + fseskills1);
        else
            $("#modifypop").html(alertMessage + late + "</br>" + tNotes + "<br>" + ssoid + "</br>" + fseskills1);
    }
    else if (gduration != $("#taskduration").val() && gES == $("#txtestart").val() && gLS == $("#txtlstart").val()) {
        jQuery('#modal-3').modal('show');

        if (gTaskNotes == $("#tasknotes").val())
            $("#modifypop").html(alertMessage + task + "<br>" + ssoid + "</br>" + fseskills1);
        else
            $("#modifypop").html(alertMessage + task + "</br>" + tNotes + "<br>" + ssoid + "</br>" + fseskills1);
    }
    else if (gduration == $("#taskduration").val() && gES != $("#txtestart").val() && gLS == $("#txtlstart").val()) {
        jQuery('#modal-3').modal('show');
        if (gTaskNotes == $("#tasknotes").val())
            $("#modifypop").html(alertMessage + sart + "<br>" + ssoid + "</br>" + fseskills1);
        else
            $("#modifypop").html(alertMessage + sart + "</br>" + tNotes + "<br>" + ssoid + "</br>" + fseskills1);
    }
    else if (gduration != $("#taskduration").val() && gES != $("#txtestart").val() && gLS != $("#txtlstart").val()) {
        jQuery('#modal-3').modal('show');
        if (gTaskNotes == $("#tasknotes").val())
            $("#modifypop").html(alertMessage + task + "</br>" + sart + "<br>" + late + "<br>" + ssoid + "</br>" + fseskills1);
        else
            $("#modifypop").html(alertMessage + task + "</br>" + sart + "<br>" + late + "</br>" + tNotes + "<br>" + ssoid + "</br>" + fseskills1);
    }
    else if (gduration == $("#taskduration").val() && gES == $("#txtestart").val() && gLS == $("#txtlstart").val() && SSOID1Validate == true) {
        jQuery('#modal-3').modal('show');
        if (gTaskNotes == $("#tasknotes").val())
            $("#modifypop").html(alertMessage + ssoid + "</br>" + fseskills1);
        else
            $("#modifypop").html(alertMessage + tNotes + "</br>" + ssoid + "</br>" + fseskills1);
    }
    else if (fseskills != $("#Skillfse1").val() && gduration == $("#taskduration").val() && gES == $("#txtestart").val() && gLS == $("#txtlstart").val() && SSOID1Validate != true) {

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
        //$("#modifypop").html("No Changes made to existing task. Do you want to modify the task?");

        jQuery('#modal-3').modal('hide');
        jQuery('#modal-modifysitesystem').modal('hide');
        $('#modifytask').click();

    }
    // jQuery('#modal-3').modal('show');

}

/* Part pickup modify popup for YES*/
$("#modifyTaskModel").click(function (e) {
    var actvityDep = $("#checktaskid").val();
    if (GetSiteSystemData.SiteCountActual > 0 || GetSiteSystemData.SystemCountActual > 0) {
        SiteCountActualflag = true;
        jQuery('#modal-modifysitesystem').modal('show');
        var alertMessageDep = "There is a dependency job exist with " + actvityDep + ", please confirm whether you want to modify appointments for this job ? " + '<br>';

        // var alertMessageDep = "Dependency Job exists,Do you want to proceed with modify of both main and dependency(s) job? " + '<br>';
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

    //if ($("#taskduration").val() == 0 || $("#taskduration").val() == null) {
    //    jQuery('#modal-2').modal('show')
    //    if ($('#hsrType').val() != "Installation") {
    //        $("#text").html("Visit Duration can't be zero. Please enter...")
    //    }
    //    else {
    //        $("#text").html("Task Duration can't be zero. Please enter...")
    //    }
    //    $('#taskduration').focus();
    //    return true;
    //}

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
        //alert("fvfd");


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
        // jQuery('#modal-3').modal('show');
        jQuery('#modal-modifyPart1').modal('hide')
    }

    if (partjobExitflag != true) {
        if (GetSiteSystemData.SiteCountActual > 0 || GetSiteSystemData.SystemCountActual > 0) {
            var actvityDep = $("#checktaskid").val();
            SiteCountActualflag = true;
            jQuery('#modal-modifysitesystem').modal('show');

            var alertMessageDep = "There is a dependency job exist with " + actvityDep + ", please confirm whether you want to modify appointments for this job ? " + '<br>';
            //    var alertMessageDep = "Dependency Job exists,Do you want to proceed with modify of both main and dependency(s) job? " + '<br>';
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
//actvityDetails = @Session["checktaskids"];

$("#btnCancelTask").click(function (e) {


    //var arrSite = GetSiteSystemData.SiteStatus;
    //if (arrSite != null || arrSite != undefined) {
    //    if (arrSite.indexOf(',') != 0) {

    //        var myarray = arrSite.split(',');
    //        for (var i = 0; i < myarray.length; i++) {

    //            if (myarray[i] != "Completed") {
    //                SiteStat = myarray[i];
    //            }
    //            else {
    //                SiteStat = myarray[i];
    //            }
    //        }

    //    }
    //}

    //var arrSystem = GetSiteSystemData.SystemStatus;
    //if (arrSystem != null || arrSystem != undefined) {

    //    if (arrSystem.indexOf(',') != 0) {

    //        var myarray = arrSystem.split(',');
    //        for (var i = 0; i < myarray.length; i++) {

    //            if (myarray[i] != "Completed") {
    //                SystemStat = myarray[i];
    //            }
    //        }
    //    }
    //}

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
        //jQuery('#modal-1').modal('show');
    }

    //&& (SystemStat != "Completed" || SiteStat == "Completed")
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
        //jQuery('#modal-1').modal('show');
    }

    if (depPartFlag == false && depSysFlag == false) { jQuery('#modal-1').modal('show'); }

})

$("#canclTaskModel").click(function (e) {

    if (depSysFlag == true) {
        jQuery('#modal-Depnedency').modal('hide');
        depSysFlag = false;
    }

    if (depPartFlag == true) {
        //jQuery('#modal-Part1').modal('hide');

    }
    else { jQuery('#modal-1').modal('show'); }


})


$("#canclPTaskModel").click(function (e) {
    if (depPartFlag == true) {
        jQuery('#modal-Part1').modal('hide');
        depPartFlag = false;

    }
    if (depSysFlag == true) {
        // jQuery('#modal-Depnedency').modal('hide');

    }
    else {
        jQuery('#modal-1').modal('show');
    }

})
/*Modify Site System Dependency Popup's*/
//$('#NoPartPickup').click(function (e) {
//    
//    if (GetSiteSystemData.SiteCountActual > 0 || GetSiteSystemData.SystemCountActual > 0) {
//        jQuery('#modal-modify').modal('show');
//        //var alertMessageDep = "Dependency Job exists, please de-link the Activity # " + actvityDetails + " (Task ID) in SDT Schedule. Do you want to proceed with modify of both main and dependency(s) job? " + '<br>';
//        var alertMessageDep = "Dependency Job exists, please de-link the Activity in SDT Schedule. Do you want to proceed with modify of both main and dependency(s) job? " + '<br>';
//        jQuery('#modal-1').modal('hide');
//        $("#modifypop").html(alertMessageDep);
//    }
//});

$('#ModifyYesDepTask').click(function () {


    jQuery('#modal-modifyPart1').modal('hide')
    jQuery('#modal-modifysitesystem').modal('hide');
    ModifyMainAndPartValidations();
});
/*Modify Site System Dependency Popup's*/


$("#modifytask").click(function (e) {
    // alert("svsfv");
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
        // alert(ID1);
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
            //if (addr2[i][j][4] == "KOREA, REPUBLIC OF") {

            //    addr2[i][j][4] = "KOREA REPUBLIC OF";
            //}
            if (addr2[i][j][4] == "South Korea") {

                addr2[i][j][4] = "KOREA, REPUBLIC OF";
            }
            //Before Modify Check whether Customer site selected or not
            //if (addr2[i][j][0] == "Customer Site") {
            //    flg=true
            //}
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
        //tasknotes = tasknotes.replace(/\n/g, "\\n");

        if (tasknotes.indexOf(',') != -1) {
            tasknotes = tasknotes.replace(/,/g, "");
        }
    }
    e.preventDefault();
    // Changes done by Raju, send array to controller intead of string

    if ($("#hsrType").val() == "Installation") {
        var dataToPost = {
            //string EStart, string LStart, int Duration, string TaskNotes, string addressArray, string fseSkill, string partcomments,string taskOpenDate,string jobType         

            earlyStart: $("#txtestart").val(),
            lateStart: "",
            duration: $("#htDurationInstall").val().toString(),
            addressArray: addr2,
            TaskNotes: tasknotes,
            fseSkill: $("#Skillfse1").val() + "," + $("#Skillfse2").val() + "," + $("#Skillfse3").val(),
            PreferredFSEs: ID1,
            JobType: "Install",
            partcomments: $('#txtPartComments').val(),
            IsRequiredfse: isrequiredfse
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
            IsRequiredfse: isrequiredfse
        };
    }

    //if (flg == true)
    //    {
    $.ajax({
        url: '../RequestAppointmentBooking/ModifyVisit',
        type: "POST",
        data: dataToPost,
        success: function (data) {

            if (data.message == "Success") {
                //modal reload added by Phani Kanth P.
                jQuery('#modal-Reload').modal('show')
                //  jQuery('#modal-2').modal('show')
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
                    // jQuery('#modal-2').modal('show')
                    //modal reload added by Phani Kanth P.
                    jQuery('#modal-Reload').modal('show')
                    $("#TextReload").html("Task Modified Successfully.")
                }
                else {
                    jQuery('#modal-2').modal('show')
                    $("#text").html("Task Not Modified. (Exception: " + data.Message + ")")
                }
            }
        },
        error: function () {
            jQuery('#modal-2').modal('show')
            $("#text").html("An error has occured while modifing the task.")

        }
    });
    return true
    //}
    //else
    //{
    //    jQuery('#modal-2').modal('show')
    //    $("#text").html("Please select Delivery Type as Customer Site in order to Modify part pick jobs.")

    //}
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
        //alert("fvfd");


    }
    else {
        if (!isDisabled) {
            var IsDeliveryDateValidationFail = DeliveryDateValidations()

            if (IsDeliveryDateValidationFail)
                return true;
        }
    }


    //if ($("#taskduration").val() == 0 || $("#taskduration").val() == null) {
    //    jQuery('#modal-2').modal('show')
    //    if ($('#hsrType').val() != "Installation") {
    //        $("#text").html("Visit Duration can't be zero. Please enter...")
    //    }
    //    else if ($("#htDurationInstall").val() == 0 || $("#htDurationInstall").val() == null) {

    //        $("#text").html("Task Duration can't be zero. Please enter...")
    //    }
    //    $('#taskduration').focus();
    //    return true;
    //}

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

    //var fseSkill = $("#Skillfse1").val() + "," + $("#Skillfse2").val() + "," + $("#Skillfse3").val();
    var s = '';
    var ID1;
    var SSOID1;

    var isrequiredfse;
    isrequiredfse = $("#chkfse1").is(":checked");
    //alert($(".selectpicker"));
    // var values = $(".selectpicker").val();

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
        //alert(ID1);
        //alert(values);
    }
    else {
        ID1 = null;
    }
    //alert(addr1.length);
    //
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
            //if (addr1[i][j][4] == "KOREA, REPUBLIC OF") {

            //    addr1[i][j][4] = "KOREA REPUBLIC OF";
            //}
            if (addr1[i][j][4] == "South Korea") {

                addr1[i][j][4] = "KOREA, REPUBLIC OF";
            }
            //Before Create Check whether Customer site selected or not
            //if (addr1[i][j][0] == "Customer Site") {
            //    flg = true
            //}
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
    //}

    s += ",";
    var tasknotes = $('#tasknotes').val();

    if (tasknotes != "") {
        //  tasknotes = tasknotes.replace(/\n/g, "\\n");

        if (tasknotes.indexOf(',') != -1) {
            tasknotes = tasknotes.replace(/,/g, "");
        }
    }
    // Changes done by Raju, send array to controller intead of string

    if ($("#hsrType").val() == "Installation") {
        var dataToPost = {
            //string EStart, string LStart, int Duration, string TaskNotes, string addressArray, string fseSkill, string partcomments,string taskOpenDate,string jobType
            Duration: $("#htDurationInstall").val(),
            TaskNotes: tasknotes,
            addressArray: addr1,
            fseSkill: fseSkill,
            PreferredFSEs: ID1,
            partcomments: $('#txtPartComments').val(),
            taskOpenDate: $("#txtestart").val(),
            JobType: "Install",
            IsRequiredfse: isrequiredfse
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
            IsRequiredfse: isrequiredfse
        };
    }
    //
    e.preventDefault(); // <------------------ stop default behaviour of button
    var element = this;

    //if (flg == true) {
    $.ajax({
        url: '../RequestAppointmentBooking/ProcessTaskwithoutAppointment',
        type: "POST",
        data: dataToPost,
        success: function (data) {
            if (data.message == "Success" && data.JobType == "Part") {
                //alert(" Task # " + data.TaskID + " and Part Pick Up Job have been created successfully.");
                jQuery('#modal-Reload').modal('show')
                $("#TextReload").html(" Task # " + data.TaskID + " and Part Pick Up Job have been created successfully.")//Changes By ankur
                $("#btnCancelTask").enabled = true;
                $("#modVis").visible = true;
                $("#creVisNA").visible = false;
                //location.reload();
                //<------------ submit form
            }
            else {
                if (data.message == "Success") {
                    //alert("Task No: " + data.TaskID + " has been created successfully.");
                    jQuery('#modal-Reload').modal('show')
                    $("#TextReload").html("Task No: " + data.TaskID + " has been created successfully.");
                    $("#btnCancelTask").enabled = true;
                    $("#modVis").visible = true;
                    $("#creVisNA").visible = false;
                    //location.reload();

                }
                else {
                    //alert("in error part");
                    jQuery('#modal-2').modal('show')
                    $("#text").html("Error occurs while creating the Task! (Exception:" + data.message + ")")
                    //alert("Error occurs while creating the Task!");
                }
            }

        },
        error: function () {
            jQuery('#modal-2').modal('show')
            $("#text").html("An error has occured!!!")
            //alert("An error has occured!!!");
        }
    });
    //}
    //else {
    //    jQuery('#modal-2').modal('show')
    //    $("#text").html("Please select Delivery Type as Customer Site in order to Create part pick jobs.")

    //}
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

                // $(this).val('');
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

                // $(this).val('');
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
    // if ($('#hsrType').val() != "Installation")
    // {
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



        //var today = new Date();
        //var todayMonth = today.getMonth() + 1;
        //var todayDay = today.getDate();
        //var todayYear = today.getFullYear();
        //var todayDateText = todayMonth + "/" + todayDay + "/" + todayYear;
        //var Currentdate = Date.parse(todayDateText);
        //var dtEarlyStart = new Date(selectedEStart).getTime();
        //var dtLateStart = new Date(selectedLStart).getTime();
        //var dtToday = new Date(today).getTime();
        //Current date validation with lLocal asset time
        var Currentdate = $("#tdefaultDate").val();
        if (Currentdate != "") {
            if (Date.parse(Currentdate.split("/")[1] + "/" + Currentdate.split("/")[0] + "/" + Currentdate.split("/")[2].substr(0, 4)) > Date.parse(EStart.split("/")[1] + "/" + EStart.split("/")[0] + "/" + EStart.split("/")[2].substr(0, 4))) {
                //if (new Date().getTime() > new Date(LStart).getTime())
                jQuery('#modal-2').modal('show');
                $("#text").html("Early Start Date Should Be Beyond Today's Date.");
                $('#txtestart').focus();
                return true;
            }
            if ($('#hsrType').val() != "Installation") {
                if (Date.parse(Currentdate.split("/")[1] + "/" + Currentdate.split("/")[0] + "/" + Currentdate.split("/")[2].substr(0, 4)) > Date.parse(LStart.split("/")[1] + "/" + LStart.split("/")[0] + "/" + LStart.split("/")[2].substr(0, 4))) {
                    //if (new Date().getTime() > new Date(LStart).getTime())
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

                    //var date = new Date(EStart.split("/")[1] + "/" + EStart.split("/")[0] + "/" + EStart.split("/")[2].substr(0, 4));
                    //var newdate = new Date(date);
                    //newdate.setDate(newdate.getDate() + 29);

                    //var todayMonth1 = newdate.getMonth() + 1;
                    //var todayDay1 = newdate.getDate();
                    //var todayYear1 = newdate.getFullYear();
                    //var todayDateText = todayMonth1 + "/" + todayDay1 + "/" + todayYear1;
                    //$("#txtlstart").val(todayDateText);


                    ////$("#txtlstart").val() = (new Date($("#txtestart").val() + 29);
                    ////$("#txtlstart").val() = new Date(EStart.split("/")[0] +  29 + "/" + EStart.split("/")[1] + "/" + EStart.split("/")[2]);
                    ////var cur = new Date(EStart.split("/")[1] + "/" + EStart.split("/")[0] + "/" + EStart.split("/")[2]);
                    ////now.setDate(now.getDate() + 30);

                    ////$("#txtlstart").val() = cur.setDate(cur.getDate() + 30);
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
            //var Etime = EStart.split(" ")[1];
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

    //if (today > EStart)
    //   {
    //    jQuery('#modal-2').modal('show');
    //    $("#text").html("Task Start Date Time should not be less than the Current Date,Please select correct date ");
    //    $('#txtestart').focus();
    //    return true;
    //}
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
        //traditional: true,
        //contentType: "application/json; charset=utf-8",
        success: function (data) {
            //
            if (data.message == "Cancelled") {
                //alert("Task Cancelled Sucessfully");
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
                //setTimeout(function () {
                //    location.reload();
                //}, 3000);
                //location.reload();
                //<------------ submit form
            }
            else if (data.message == "Completed") {
                jQuery('#modal-Reload').modal('show');
                $("#TextReload").html("Already Completed Task Cannot be Cancelled.");
            }
            else {
                jQuery('#modal-2').modal('show')
                $("#text").html("Error occured during Task Cancellation (Exception" + data.message + ")")
            }
        },
        error: function () {
            alert("An error has occured!!!");
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
    //if ($("#hsrType").val() != "Installation") {
    //var today = new Date();
    //var todayMonth = today.getMonth() + 1;
    //var todayDay = today.getDate();
    //var todayYear = today.getFullYear();
    //var todayDateText = todayMonth + "/" + todayDay + "/" + todayYear;
    //var Currentdate = Date.parse(todayDateText);
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
        //if ($("#hsrType").val() != "Installation")
        if (EStart == "") {
            jQuery('#modal-2').modal('show')
            $("#text").html("Please Enter Early Start Date.")
            $('#txtestart').focus();
            return true;
        }
        if (DeliDate != "" && Currentdate != undefined) {
            if (Date.parse(Currentdate.split("/")[1] + "/" + Currentdate.split("/")[0] + "/" + Currentdate.split("/")[2].substr(0, 4)) > Date.parse(DeliDate.split("/")[1] + "/" + DeliDate.split("/")[0] + "/" + DeliDate.split("/")[2].substr(0, 4))) {
                //if (new Date().getTime() > new Date(LStart).getTime())
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
                //else {
                //    $('#txtDelivery').focus();
                //}
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
                    //else {
                    //    $('#txtDelivery').focus();
                    //}

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
                //if ($('.editOption').val() != '') {
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
                // }
                //else {
                //    $("#imgPreferedPSEValid").hide();
                //    $("#imgPreferedPSECross").hide();
                //}
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

//function CreateReqWithoutApt()
//{
//    alert("InRequestWithoutApt");
//    window.location = "/RequestAppointmentBooking/RequestAppointment?startdate=" + $("#txtestart").val() + "&enddate=" + $("#txtlstart").val() + "&duration=" + $("#taskduration").val().toString();
//}

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
    //"startDate": "27/10/2015",
    //"endDate": "02/11/2015"
}, function (start, end, label) {
    console.log("New date range selected: ' + start.format('YYYY-MM-DD') + ' to ' + end.format('YYYY-MM-DD') + ' (predefined range: ' + label + ')");
});




//$('#txtDelivery').on('cancel.daterangepicker', function (ev, picker) {
//    //do something, like clearing an input

//});


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
            //console.log("change : " + value);
        },
        release: function (value) {
            //console.log(this.$.attr('value'));
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



