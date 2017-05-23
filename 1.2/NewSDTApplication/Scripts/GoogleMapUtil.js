var addr = [];
var addr1 = [[]];
var addr2 = [[]];
var addr3 = [[]];
var countdata;
var curenrRecord;
var totalrec;
var customerMarkerAdded = false;
var isCustomerDeliverTypeSelected = false;
var addressBuilder1 = "";
var markers = [];
var gmarkers = [];
//var infowindow = new google.maps.InfoWindow();
var PartDataModified = false;
$(document).ready(function () {


    $('#deliverytype').attr("disabled", "disabled");
    $('#address').attr("disabled", "disabled");
    $('#city').attr("disabled", "disabled");
    $('#postcode').attr("disabled", "disabled");
    $('#txtPartComments').attr("disabled", "disabled");
    $('#mySelect').attr("disabled", "disabled");
    $('#checkaddress').attr("disabled", "disabled");
    $('#txtDelivery').attr("disabled", "disabled");
    updateStatus();

    //Blink effects added by Phani Kanth P.
    $(function () {
        blinkeffect('#txtblnk');
    })
    function blinkeffect(selector) {
        $(selector).fadeOut('slow', function () {
            $(this).fadeIn(3000, function () {
                blinkeffect(this);
            });
        });
    }

    $("#checkpartcheckclick").hide();
    // barnali-cross
    $('.modal-content .modal-header #canaddress').click(function () {

        if ($('#postcode').val() != "" && $('#deliverytype :selected').text() != "" && $("#countdata").text() == "Creation") {
            DeleteMarker($('#postcode').val(), $('#deliverytype :selected').text());
        }
        var $confirm = $("#CheckAddresspopup");
        $confirm.modal("hide");
    });

    $.ajax({
        type: "GET",
        url: "../XML/Countries.xml",
        dataType: "xml",
        success: function (xml) {

            var select = $('#mySelect');
            $(xml).find('Countries').each(function () {
                $(this).find('Country').each(function () {
                    var value = $(this).text();
                    select.append("<option class='ddindent' value='" + value + "'>" + value + "</option>");
                });
            });
            select.children(":first").text("--Please Select--").attr("selected", true);
            InitOldAddress();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("some error");
        }
    });

    //model
    var ShippingAddressImage = '../Images/Warehouse.png';
    var OfficeAddressImage = '../Images/Warehouse.png';
    var WarehouseAddressImage = '../Images/Warehouse.png';
    var SiteAddressImage = '../Images/Hopital.png';

    // Define the Person constructor
    var Address = function (address, city, postcode, country, addressType) {
        this.address = address;
        this.city = city;
        this.postcode = postcode;
        this.country = country;
        this.addressType = addressType;
    };


    var map;
    var elevator;

    var myLatlng = new google.maps.LatLng(0, 0); // Add the coordinates
    var mapOptions = {
        zoom: 12, // The initial zoom level when your map loads (0-20)
        minZoom: 0, // Minimum zoom level allowed (0-20)
        maxZoom: 20, // Maximum soom level allowed (0-20)
        zoomControl: true, // Set to true if using zoomControlOptions below, or false to remove all zoom controls.
        zoomControlOptions: {
            style: google.maps.ZoomControlStyle.DEFAULT // Change to SMALL to force just the + and - buttons.
        },
        center: myLatlng, // Centre the Map to our coordinates variable
        mapTypeId: google.maps.MapTypeId.ROADMAP, // Set the type of Map
        scrollwheel: false, // Disable Mouse Scroll zooming (Essential for responsive sites!)
        // All of the below are set to true by default, so simply remove if set to true:
        panControl: false, // Set to false to disable
        mapTypeControl: false, // Disable Map/Satellite switch
        scaleControl: false, // Set to false to hide scale
        streetViewControl: false, // Set to disable to hide street view
        overviewMapControl: false, // Set to false to remove overview control
        rotateControl: false // Set to false to disable rotate control
    }
    map = new google.maps.Map($('#map_canvas')[0], mapOptions);
  

    var addr = [];
    var isValid, isValidaddr, isValidpostcode, isValidcity, isaddressValid, iscountryValid = true;

    //var addressBuilder1 = "";
    

    $("#checkaddress").click(function (event) {

        event.preventDefault();
       event.stopImmediatePropagation();

        var EStart = $("#txtestart").val();
        var dDate = $("#txtDelivery").val();
        var partComments = $("#txtPartComments").val();
        var address = new Address($('#address').val(), $('#city').val(), $('#postcode').val(), $('#mySelect :selected').text(), $('#deliverytype :selected').text());
        //TODO: PALANI CODE STARTED FOR UI VALIDATIONS
        if (address.address == null || address.address == undefined || address.address.trim() == "" || $('#address').val() == null || $('#address').val() == undefined || $('#address').val().trim() == "") {
            isValidaddr = false;
            $('#address').css({
                "border": "1px solid red",
                "background": "#FFCECE"
            });
            $("#addressS").hide();
            $("#addressE").show();
        }
        else {
            isValidaddr = true;
            $('#address').css({
                "border": "",
                "background": ""
            });
            $("#addressS").show();
            $("#addressE").hide();
        }

        if ($('#mySelect :selected').val() == "KOREA, REPUBLIC OF") {

            if (isNaN(address.postcode)) {
                $('#postcode').css({
                    "border": "1px solid red",
                    "background": "#FFCECE"
                });
                $("#postcodeE").show();
                $("#postcodeS").hide();
                isValidaddr = false;
            }
            else {
                $('#postcode').css({
                    "border": "",
                    "background": ""
                });
                $("#postcodeS").show();
                $("#postcodeE").hide();
                isValidpostcode = true;
            }
        }
        else {

            if (isNotZipCode(address.postcode) || isNaN(address.postcode) || address.postcode == null || address.postcode == undefined || address.postcode == "" || $('#postcode').val() == null || $('#postcode').val() == undefined || $('#postcode').val() == "") {
                isValidpostcode = false;
                $('#postcode').css({
                    "border": "1px solid red",
                    "background": "#FFCECE"
                });
            }
            else {
                isValidpostcode = true;
                $('#postcode').css({
                    "border": "",
                    "background": ""
                });
            }
        }


        if ($('#mySelect :selected').val() == "KOREA, REPUBLIC OF") {


            $('#city').css({
                "border": "",
                "background": ""
            });
            $("#cityS").show();
            $("#cityE").hide();
            isValidcity = true;


        }

        else {

            if (address.city.trim() == "" || !isNaN(address.city)) {
                isValidcity = false;
                $("#cityE").show();
                $("#cityS").hide();
                $('#city').css({
                    "border": "1px solid red",
                    "background": "#FFCECE"
                });
            }
            else {
                isValidcity = true;
                $("#cityE").hide();
                $("#cityS").show();
                $('#city').css({
                    "border": "",
                    "background": ""
                });
                $("#cityE").hide();
                $("#cityS").show();
            }
        }


        //if (address.city == null || address.city == undefined || address.city.trim() == "") {
        //    isValidcity = false;
        //    $('#city').css({
        //        "border": "1px solid red",
        //        "background": "#FFCECE"
        //    });
        //}
        //else {
        //    isValidcity = true;
        //    $('#city').css({
        //        "border": "",
        //        "background": ""
        //    });
        //}
        if (address.country == null || address.country == undefined || address.country == "" || address.country == "--Please Select--") {
            iscountryValid = false;
            $('#mySelect').css({
                "border": "1px solid red",
                "background": "#FFCECE"
            });
        }
        else {
            iscountryValid = true;
            $('#mySelect').css({
                "border": "",
                "background": ""
            });
        }



        if (valueSelected == "") {
            var optionSelected = $('#deliverytype').find("option:selected");
            valueSelected = optionSelected.val();
        }


        if (address.addressType == null || address.addressType == undefined || address.addressType == "--Please Select--" || valueSelected == null || valueSelected == undefined || CountryvalueSelected == null || CountryvalueSelected == undefined) {
            isaddressValid = false;
            $('#deliverytype').css({
                "border": "1px solid red",
                "background": "#FFCECE"
            });
        }
        else {
            isaddressValid = true;
            $('#deliverytype').css({
                "border": "",
                "background": ""
            });
        }

        // CODE FINISHED
        if (isValidaddr && iscountryValid && isaddressValid && isValidpostcode && isValidcity) {
            //No need to replace, code Removed
            //if (address.address.indexOf(',') != -1) {
            //    address.address = address.address.replace(/,/g, "");
            //}
            //if (address.city.indexOf(',') != -1) {
            //    address.city = address.city.replace(/,/g, "");
            //}
            //if (address.country.indexOf(',') != -1) {
            //    address.country = address.country.replace(/,/g, "");
            //}
            //address.city = address.city.replace(/=/g, "");
            //address.address = address.address.replace(/=/g, "");

            checkAddress(address);
            // isValid = true;
        }
    });

  

    $('#imgplus').click(function () {
        $("#postcodeS").hide(); $("#postcodeE").hide(); $("#cityE").hide(); $("#cityS").hide(); $("#addressE").hide(); $("#addressS").hide();
        valueSelected = ""; dDate = "";
        if ($('#checkStatval').val() != "Cancelled" && $('#address').val() == "") {
            $('#txtDelivery').val($("#tdefaultDate").val());
        }
        //$('#txtDelivery').attr('value', '');
        $("#checkpartcheckclick").hide();
    });

    $('#imgdel').click(function () {
        $("#postcodeS").hide(); $("#postcodeE").hide(); $("#cityE").hide(); $("#cityS").hide(); $("#addressE").hide(); $("#addressS").hide();
        $("#checkpartcheckclick").hide();
    });

    var CountryvalueSelected = "";
    $('#mySelect').change(function () {
        var CountryoptionSelected = $(this).find("option:selected");
        CountryvalueSelected = CountryoptionSelected.val();
        var CountrytextSelected = CountryoptionSelected.text();
        if (CountryvalueSelected != null || CountryvalueSelected != undefined || CountryvalueSelected != "" || CountryvalueSelected != "--Please Select--") {
            $('#mySelect').css({
                "border": "",
                "background": ""
            });
        }
        else {
            $('#mySelect').css({
                "border": "1px solid red",
                "background": "#FFCECE"
            });
        }

        if ($('#mySelect :selected').val() == "KOREA, REPUBLIC OF") {
            if (isNaN($('#postcode').val())) {
                $('#postcode').css({
                    "border": "1px solid red",
                    "background": "#FFCECE"
                });
                $("#postcodeE").show();
                $("#postcodeS").hide();
            }
            else {
                $('#postcode').css({
                    "border": "",
                    "background": ""
                });
                $("#postcodeS").show();
                $("#postcodeE").hide();
            }

        }
        else {
            if (isNotZipCode($('#postcode').val()) || isNaN($('#postcode').val()) || $('#postcode').val() == null || $('#postcode').val() == undefined || $('#postcode').val() == "") {
                isValidpostcode = false;
                $('#postcode').css({
                    "border": "1px solid red",
                    "background": "#FFCECE"
                });
                $("#postcodeS").hide();
                $("#postcodeE").show();
            }
            else {
                isValidpostcode = true;
                $('#postcode').css({
                    "border": "",
                    "background": ""
                });
                $("#postcodeS").show();
                $("#postcodeE").hide();
            }
        }
        if ($('#mySelect :selected').val() == "KOREA, REPUBLIC OF") {


            $('#city').css({
                "border": "",
                "background": ""
            });
            $("#cityS").show();
            $("#cityE").hide();
            isValidpostcode = true;

        }

        else {

            if ($('#city').val().trim() == "" || !isNaN($('#city').val())) {
                $("#cityE").show();
                $("#cityS").hide();
                $('#city').css({
                    "border": "1px solid red",
                    "background": "#FFCECE"
                });
                isValidpostcode = false;
            }
            else {
                isValidpostcode = true;
                $("#cityE").hide();
                $("#cityS").show();
                $('#city').css({
                    "border": "",
                    "background": ""
                });
                $("#cityE").hide();
                $("#cityS").show();
            }
        }


    });

    // Showing Default Customer site on Map along with other delivery types.   

    if ($('#hstreet').val() != "" && $('#hcity').val() != "" && $('#hcountry').val() != "" && $('#hpostcode').val() != "" && $('#hlatitude').val() != "" && $('#hlongitude').val() != "") {
        // $('#latitude').val($('#hlatitude').val());
        //$('#longitude').val($('#hlongitude').val());

        var hstreet = $('#hstreet').val();
        hstreet = hstreet.trim();
        $('#hstreet').val(hstreet);
        var hcity = $('#hcity').val();
        var hcountry = $('#hcountry').val();
        var hpostcode = $('#hpostcode').val();
        //No need to replace, code Removed
        //if (hstreet.indexOf(',') != -1) {
        //    hstreet = hstreet.replace(/,/g, "");
        //}

        //if (hcity.indexOf(',') != -1) {
        //    hcity = hcity.replace(/,/g, "");
        //}

        //if (hcountry.indexOf(',') != -1) {
        //    hcountry = hcountry.replace(/,/g, "");
        //}
        if (hcountry == "KOREA REPUBLIC OF" || hcountry == "KOREA, REPUBLIC OF") {
            hcountry = "South Korea";
        }

        //addressBuilder1 = hstreet + " " + hcity + " " + hpostcode + " " + hcountry;
        addressBuilder1 = $('#hlatitude').val() / 1000000 + "," + $('#hlongitude').val() / 1000000;

    }

    //PALANI CODE ENDED

    var valueSelected = "";
    $('#deliverytype').change(function () {
        var optionSelected = $(this).find("option:selected");
        valueSelected = optionSelected.val();
        var textSelected = optionSelected.text();
        //PALANI CODE STARTED

        if (valueSelected != null || valueSelected != undefined || valueSelected != "" || valueSelected !== "--Please Select--") {
            $('#deliverytype').css({
                "border": "",
                "background": ""
            });
        }
        if (valueSelected != "Customer Site") {



            $("#address").val('');
            $("#postcode").val('');
            $("#city").val('');
            $("#mySelect").val('');
            $('#txtPartComments').val('');
            $('#txtPartComments').focus();
        }
        else {
            //debugger;
            //$('#txtPartComments').removeAttr("disabled", "disabled");        
            //$('#txtDelivery').removeAttr("disabled", "disabled");
            PartDataModified = true;
            $('#deliverytype').css({
                "border": "",
                "background": ""
            });
            $('#txtDelivery').css({
                "border": "",
                "background": ""
            });
            $('#address').css({
                "border": "",
                "background": ""
            });
            $('#postcode').css({
                "border": "",
                "background": ""
            });
            $('#city').css({
                "border": "",
                "background": ""
            });
            $('#mySelect').css({
                "border": "",
                "background": ""
            });
        }


        // PALANI CODE ENDED.



        if (valueSelected != "Customer Site") {
            $("#address").val('');
            $("#postcode").val('');
            $("#city").val('');
            $("#mySelect").val('');
            $('#txtPartComments').val('');
            $('#txtPartComments').focus();


        }
        else {
            //debugger;
            $('#deliverytype').attr("disabled", "disabled");
            $('#address').attr("disabled", "disabled");
            $('#city').attr("disabled", "disabled");
            $('#postcode').attr("disabled", "disabled");
            $('#txtPartComments').removeAttr("disabled", "disabled");
            $('#mySelect').attr("disabled", "disabled");
            $('#checkaddress').attr("disabled", "disabled");
            $('#txtDelivery').removeAttr("disabled", "disabled");

            $('#address').val($('#hstreet').val());
            $('#city').val($('#hcity').val());
            $('#postcode').val($('#hpostcode').val());
            $('#mySelect').val($('#hcountry').val());
            var $confirm = $('#CheckAddresspopup');
           
            $confirm.modal('hide');
            PartDataModified = true;
            var address = new Address($('#address').val(), $('#city').val(), $('#postcode').val(), $('#mySelect :selected').text(), $('#deliverytype :selected').text());
            if (address.address == "" && address.city == "" && address.country == "") {
                jQuery('#modal-2').modal('show')
                $("#text").html("No Customer Site Address");

            }
            else {
                AddRecord();
            }

            //Code Removed this is not needed /Raju
            //if (address.address != "" && address.addressType != "" && address.city != "" && address.country != "" && address.postcode != "") {
            //    //No need to replace, code Removed
            //    //if (address.address.indexOf(',') != -1) {
            //    //    address.address = address.address.replace(/,/g, "");
            //    //}
            //    //if (address.city.indexOf(',') != -1) {
            //    //    address.city = address.city.replace(/,/g, "");
            //    //}

            //    //if (address.country.indexOf(',') != -1) {
            //    //    address.country = address.country.replace(/,/g, "");
            //    //}
            //    if (address.country == "KOREA REPUBLIC OF" || address.country == "KOREA, REPUBLIC OF") {
            //        address.country = "South Korea";
            //    }
            //    //addressBuilder1 = address.address + " " + address.city + " " + address.postcode + " " + address.country;
            //    addressBuilder1 = $('#hlatitude').val() / 1000000 + "," + $('#hlongitude').val() / 1000000;

            //    checkAddress(address);
            //}

        }

    });

    $('#CheckAddresspopup').on('shown.bs.modal', function () {
        // resizeMap();

        google.maps.event.trigger(map, 'resize');
    });
        
    var isCusDelivTypeUsed = false;
    var iconImg = "";
    var language = "";
    
    //google.maps.event.addListener(marker, 'click', function() {
    //    $("#checkaddress").trigger("click");
    //});

    function UpdateMarkerCustomer(addressBuilder2, addressType, postCode) {

        $.getJSON('https://maps.googleapis.com/maps/api/geocode/json?address=' + addressBuilder2 + '&sensor=false&libraries=places&language=' + language + '', function (data) {
            // When no data fetched and the Result is Zero it's throwing exception
            // thats the reason if condition added.
            var $confirm = $("#CheckAddresspopup");
            if (data.results.length > 0) {
               // $confirm.modal('show');

                // Code written to check if address already exists 
                // if not add the address
                var pCode = addressType + "-" + postCode;
                ////DeleteMarker(postCode, addressType);
                ////DeleteMarker(postCode, addressType]);
                for (var i = 0; i < markers.length; i++) {
                    if (markers[i].postcode == pCode) {
                        isaddExistsInMapAlready = true;
                        $confirm.modal('show');
                    }
                }

                if (!isaddExistsInMapAlready) {

                    isCusDelivTypeUsed = true;

                    $('#siteaddress').val(addressBuilder2);
                    var p = data.results[0].geometry.location;
                    $('#latitude').text(p.lat);
                    $('#longitude').text(p.lng);



                    var latval;
                    var longval;

                    latval = p.lat * 1000000;
                    latval = Math.round(latval);


                    longval = p.lng * 1000000;
                    longval = Math.round(longval);

                    document.getElementById("latitude").value = latval;
                    document.getElementById("longitude").value = longval;

                    var latlng = new google.maps.LatLng(p.lat, p.lng);


                    if (addressType == 'Warehouse' || addressType == "Drop Point")
                        iconImg = WarehouseAddressImage;
                    else if (addressType == 'FE Address' || addressType == 'GE Office' || addressType == 'Other')    //if (Address.addressType == 'FE Address' || Address.addressType == 'GE Office' || Address.addressType == 'Other') {
                        iconImg = ShippingAddressImage;
                    else
                        iconImg = SiteAddressImage;

                    var Shippingaddress = new google.maps.Marker({
                        position: latlng,
                        icon: iconImg,//Change made by ankur
                        map: map

                    });
                   // DeleteMarker(postCode, addressType);
                    addMarker(Shippingaddress, postCode, addressType);

                    // Push your newly created marker into the array:
                    var data1 = addressType + "-" + postCode;
                    Shippingaddress.postcode = data1;

                  //  gmarkers.push(Shippingaddress);

                    var position = new google.maps.LatLng(p.lat)
                    map.panTo(Shippingaddress.getPosition());
                    map.setCenter(Shippingaddress.getPosition(), 10);

                    //var infowindow =   new google.maps.InfoWindow({
                    //    content: Address.addressType + ":  " + data.results[0].formatted_address
                    //})
                    var infowindow = new google.maps.InfoWindow();
                    infowindow.setContent(addressType + ":  " + data.results[0].formatted_address);
                   // infowindow.position(position);
                    infowindow.open(map, Shippingaddress);
                    //google.maps.event.addListener(Shippingaddress, "click", function (e) {
                    //    //Create and open InfoWindow.
                    //    var infoWindow = new google.maps.InfoWindow();
                    //    //Wrap the content inside an HTML DIV in order to set height and width of InfoWindow.
                    //    infowindow.setContent(addressType + ":  " + data.results[0].formatted_address);
                    //    infoWindow.open(map, Shippingaddress);
                    //});
                    // Changes made by sudhir to show the marker first time in IE Browser
                  
                    //setTimeout(
                    //    function () {
                    //        infowindow.close();
                    //        infowindow.open(map, Shippingaddress);
                    //    }, 30);

                    //resizeMap();
                    //$('#myModal').on('show.bs.modal', function () {
                    //    resizeMap();
                    //})
                    // $confirm.modal('show');


                    // google.maps.event.trigger(map, 'resize');
                    //$confirm.modal('hide');

                  $confirm.modal('show');
                }


            }
            else {
                // when the map unable to fetch address details 
                // handling this by showing the pop up message as Invalid address.
                if (data.status == "ZERO_RESULTS") {
                    $('#btnYesConfirmYesNo').attr("disabled", "disabled");
                    // $invAddDlg.html("Invalid Address");
                    jQuery('#modal-2').modal('show')
                    $("#text").html("Invalid Address: " + addressType + ". Please Enter Correct Address");
                    //$invAddDlg.modal('show');
                   // $confirm.modal('hide');
                }

                //alert("Invalid Address");
            }
             isaddExistsInMapAlready = false;

        });
    }

    var isaddExistsInMapAlready = false;
    var addr3 = [[]];


    function checkAddress(Address) {
        //addr3 = addr1;
        var $confirm = $("#CheckAddresspopup");
        $('#btnYesConfirmYesNo').removeAttr("disabled");
        //Added by viji,this for demo please re-change  for customer site there is not check address so we have to fetch from addr1 array
        // if (valueSelected == "Customer Site" || valueSelected == "Customer Site") {
        // function AddorUpdatePartAddress(deliverytype, address, city, postcode, countryData, partComment, deliveryDate, latitude, longitude) {

        //  var deliverytypeold
        //Added by Rajubabu to avoid duplicate addresses showing in Map
        var latitude = '';

        var longitude = '';

        var partComment = $("#txtPartComments").val();

        var deliveryDate = $("#txtDelivery").val();


        //var latlong = '-27.418703,153.055333'
        $('#addressS').hide();
        $('#postcodeS').hide();
        $("#cityS").hide();
        var siteaddress = "";
        var CustomerSite = "";   //Added by Barnali , got null exception as CustomerSite not defined.
        var shippingaddress = ""
        var lang = navigator.language ? navigator.language[0] : (navigator.language || navigator.userLanguage);
        //alert(lang);
        //if (lang == 'ko-KR' || lang == 'ko') {
        //    siteaddress = "??? ??  :";
        //    shippingaddress = "?? ??  :";
        //}
        //else {
        siteaddress = "Site Address :";
        shippingaddress = "Shipping Address :";
        //}
        //var addressBuilder = Address.address + " " + Address.city + " " + Address.postcode + " " + Address.country;      

        var addressBuilder = "";
        
        if (Address.country == "KOREA, REPUBLIC OF" || Address.country == "KOREA REPUBLIC OF"||  Address.country == "South Korea") {
            Address.country = "South Korea";
            lang = 'ko';
            addressBuilder = Address.address + " " + Address.city + " " + Address.postcode + " " + Address.country;
            //  addressBuilder = "서울특별시 강남구 학동로 343 (논현동 포바강남타워)";
        }
        else {
            lang = 'en';
            addressBuilder = Address.address + " " + Address.city + " " + Address.postcode + " " + Address.country;
        }

        // addressBuilder = addressBuilder.replace(/#/g, "");

        var image;
        if (Address.addressType == 'Customer Site') {
            // To hide the images of validations when selecting the customer site in the dropdown delivery type.
            $("#postcodeS").hide(); $("#postcodeE").hide(); $("#cityE").hide(); $("#cityS").hide(); $("#addressE").hide(); $("#addressS").hide();

            isCustomerDeliverTypeSelected = true;

            //alert(isCustomerDeliverTypeSelected);

            // setTimeout(function () {
            // }, 50);

        }
               
//Modify Address fro part Delivery type if already address is available 
        for (var i = 0; i < addr3[addr3.length - 1].length; i++) {
            DeleteMarker(addr3[0][i][3], addr3[0][i][0]);

            if (addr3[0][i][0] == Address.addressType) {
              // DeleteMarker(addr3[0][i][3], addr3[0][i][0]);

                addr3[0].splice(i, 1);
            }
        }

        // markers = [];
        addr3 = [[]];
        for (var j = 0; j < addr1[addr1.length - 1].length; j++) {
            //Delete all old markers
            DeleteMarker(addr1[0][j][3], addr1[0][j][0]);

            if (addr1[0][j][0] != "Customer Site") {
                if (addr1[0][j].length > 10 && addr1[0][j][10] != "Cancelled") {
                    addr3[addr3.length - 1].push([
                    addr1[0][j][0],
                    addr1[0][j][1],
                    addr1[0][j][2],
                    addr1[0][j][3],
                    addr1[0][j][4],
                    addr1[0][j][5],
                    addr1[0][j][6],
                    addr1[0][j][7],
                    addr1[0][j][8],
                    addr1[0][j][9],
                     addr1[0][j][10]
                    ]);
                }
                else if (addr1[0][j].length == 9) {
                    addr3[addr3.length - 1].push([
                     addr1[0][j][0],
                     addr1[0][j][1],
                     addr1[0][j][2],
                     addr1[0][j][3],
                     addr1[0][j][4],
                     addr1[0][j][5],
                     addr1[0][j][6],
                     addr1[0][j][7],
                     addr1[0][j][8]

                    ]);

                }
            }
        }



        for (var i = 0; i < addr3[addr3.length - 1].length; i++) {

            if (addr3[0][i].length > 10 && addr3[0][i][0] == Address.addressType) {
                if (addr3[0][i][0] == Address.addressType && (addr3[0][i][10] == "New" || addr3[0][i][10] == "Tentative")) {

                    //deleteold marker
                    DeleteMarker(addr3[0][i][3], addr3[0][i][0]);

                    addr3[0].splice(i, 1);

                }
            }
            else if (addr3[0][i][0] == Address.addressType && addr3[0][i].length == 9) {
                //deleteold marker
                DeleteMarker(addr3[0][i][3], addr3[0][i][0]);

                addr3[0].splice(i, 1);

            }
        }

        if (Address.addressType != "Customer Site") {
            addr3[addr3.length - 1].push([Address.addressType, Address.address, Address.city, Address.postcode, Address.country, partComment, deliveryDate, latitude, longitude]);
        }

        if (addr3[0].length > 0) {
            for (var i = 0; i < addr3[addr1.length - 1].length; i++) {

                //if (addr1[0][i][0] == 'Customer Site') {

                //  var addressBuilder = Address.address + " " + Address.city + " " + Address.postcode + " " + Address.country;
                // Remove Commas if exists.

                //No need to replace, code Removed
                //if (addr3[0][i][1].indexOf(',') != -1) {
                //    addr3[0][i][1] = addr3[0][i][1].replace(/,/g, "");
                //}
                //if (addr3[0][i][2].indexOf(',') != -1) {
                //    addr3[0][i][2] = addr3[0][i][2].replace(/,/g, "");
                //}

                //if (addr3[0][i][4].indexOf(',') != -1) {
                //    addr3[0][i][4] = addr3[0][i][4].replace(/,/g, "");
                //}
                //Set korea language
                if (addr3[0][i][4] == "KOREA REPUBLIC OF" || addr3[0][i][4] == "KOREA, REPUBLIC OF"|| addr3[0][i][4] == "South Korea") {
                    addr3[0][i][4] = "South Korea";
                    language = 'ko';
                }
                else {
                    language = 'en';
                }
                addressBuilder = addr3[0][i][1] + " " + addr3[0][i][2] + " " + addr3[0][i][3] + " " + addr3[0][i][4];


                //addressBuilder = addressBuilder.replace(/#/g, "");

                //DeleteMarker(addr1[0][i][3], addr1[0][i][0]);
                UpdateMarkerCustomer(addressBuilder, addr3[0][i][0], addr3[0][i][3]);
                //customerMarkerAdded = true;
                //}
            }
            //}

        }

        //when  if (addr1[0].length == 0 Map null 
       
       
        //<script src="//maps.googleapis.com/maps/api/js?v=3.exp&sensor=false&libraries=places&language=ko"></script>
        //Get the address from Google Map
        if (Address.addressType != 'Customer Site' &&   $("#countdata").text()=="Creation") {
            //debugger;

            if (Address.addressType == 'Drop Point' || Address.addressType == 'Warehouse') {
                $.getJSON('https://maps.googleapis.com/maps/api/geocode/json?address=' + addressBuilder + '&sensor=false&libraries=places&language=' + lang + '', function (data) {
                    if (data.results.length > 0) {
                       
                        // Code written to check if address already exists 
                        // if not add the address
                        var pCode = Address.addressType + "-" + Address.postcode;
                        for (var i = 0; i < markers.length; i++) {
                            if (markers[i].postcode == pCode) {
                                isaddExistsInMapAlready = true;
                                $confirm.modal('show');

                            }
                        }

                        if (!isaddExistsInMapAlready) {


                            $('#dropbox').val(addressBuilder);
                            var p = data.results[0].geometry.location;
                            $('#latitude').text(p.lat);
                            $('#longitude').text(p.lng);


                            var latval;
                            var longval;

                            latval = p.lat * 1000000;
                            latval = Math.round(latval);


                            longval = p.lng * 1000000;
                            longval = Math.round(longval);

                            document.getElementById("latitude").value = latval;
                            document.getElementById("longitude").value = longval;

                            var latlng = new google.maps.LatLng(p.lat, p.lng);
                            var SiteAddress = new google.maps.Marker({
                                position: latlng,
                                icon: WarehouseAddressImage,
                                map: map
                            });

                            //Change made by barnali
                            addMarker(SiteAddress, Address.postcode, Address.addressType);



                            // Push your newly created marker into the array:
                            var data1 = Address.addressType + "-" + Address.postcode;
                            SiteAddress.postcode = data1;

                         //   gmarkers.push(SiteAddress);

                            var position = new google.maps.LatLng(p.lat)
                            map.panTo(SiteAddress.getPosition());
                            map.setCenter(SiteAddress.getPosition(), 10);

                            //new google.maps.InfoWindow({
                            //    content: Address.addressType + ":  " + data.results[0].formatted_address
                            //}).open(map, SiteAddress);
                            //$confirm.modal('show');

                            //var infoWindow = new google.maps.InfoWindow({
                            //    content: Address.addressType + ":  " + data.results[0].formatted_address
                            //});

                            var infowindow = new google.maps.InfoWindow();
                            infowindow.setContent(Address.addressType + ":  " + data.results[0].formatted_address);
                            infowindow.open(map, SiteAddress);
                            //                        $confirm.modal('show');
                            //Change made by ankur

                            // Changes made by sudhir to show the marker first time in IE Browser

                            //setTimeout(
                            //    function () {
                            //        infowindow.close();
                            //        infowindow.open(map, SiteAddress);
                            //    }, 30);
                            $confirm.modal('show');

                        }
                    }
                    else {
                        // when the map unable to fetch address details 
                        // handling this by showing the pop up message as Invalid address.
                        if (data.status == "ZERO_RESULTS") {
                            // $invAddDlg.html("Invalid Address");
                            //$invAddDlg.modal('show');
                            $('#btnYesConfirmYesNo').attr("disabled", "disabled");
                            jQuery('#modal-2').modal('show')
                            $("#text").html("Invalid Address: " + Address.addressType + ". Please Enter Correct Address");
                            //alert("Invalid Address");
                           // $confirm.modal('hide');
                        }
                    }

                    isaddExistsInMapAlready = false;

                });



                // }
            }
            if (Address.addressType == 'FE Address' || Address.addressType == 'GE Office' || Address.addressType == 'Other') {


                $.getJSON('https://maps.googleapis.com/maps/api/geocode/json?address=' + addressBuilder + '&sensor=false&libraries=places&language=' + lang + '', null, function (data) {
                    if (data.results.length > 0) {
                        
                        // Code written to check if address already exists 
                        // if not add the address
                        var pCode = Address.addressType + "-" + Address.postcode;
                        for (var i = 0; i < markers.length; i++) {
                            if (markers[i].postcode == pCode) {
                                isaddExistsInMapAlready = true;
                                $confirm.modal('show');

                            }
                        }

                        if (!isaddExistsInMapAlready) {

                            $('#dropbox').val(addressBuilder);
                            var p = data.results[0].geometry.location;

                            $('#latitude').text(p.lat);
                            $('#longitude').text(p.lng);

                            var latval;
                            var longval;

                            latval = p.lat * 1000000;
                            latval = Math.round(latval);


                            longval = p.lng * 1000000;
                            longval = Math.round(longval);



                            document.getElementById("latitude").value = latval;
                            document.getElementById("longitude").value = longval;

                            //alert(p.lat);
                            //alert(p.lng);

                            var latlng = new google.maps.LatLng(p.lat, p.lng);
                            var SiteAddress = new google.maps.Marker({
                                position: latlng,
                                icon: ShippingAddressImage,
                                map: map
                            });

                            addMarker(SiteAddress, Address.postcode, Address.addressType);



                            // Push your newly created marker into the array:
                            var data1 = Address.addressType + "-" + Address.postcode;
                            SiteAddress.postcode = data1;

                          //  gmarkers.push(SiteAddress);


                            //new google.maps.InfoWindow({
                            //    content: siteaddress + " " + data.results[0].formatted_address
                            //}).open(map, SiteAddress);
                            var position = new google.maps.LatLng(p.lat)
                            map.panTo(SiteAddress.getPosition());
                            map.setCenter(SiteAddress.getPosition(), 10);
                            // map.zoomIn();
                            // SiteAddress.setMap(map)
                            var infoWindow = new google.maps.InfoWindow({
                                content: Address.addressType + ":  " + data.results[0].formatted_address
                            });

                            //google.maps.event.addEventListener(infoWindow, 'domready', function () {
                            //    $('#info01').focus().zoom();
                            //});
                            infoWindow.open(map, SiteAddress);
                            //$confirm.modal('show');


                            // Changes added by sudhir to show the marker first time
                            //setTimeout(
                            //       function () {
                            //           infoWindow.close();
                            //           infoWindow.open(map, SiteAddress);
                            //       }, 30);
                            $confirm.modal('show');

                        }
                    }
                    else {
                        // when the map unable to fetch address details 
                        // handling this by showing the pop up message as Invalid address.
                        if (data.status == "ZERO_RESULTS") {
                            //$invAddDlg.html("Invalid Address");
                            // $invAddDlg.modal('show');
                            $('#btnYesConfirmYesNo').attr("disabled", "disabled");
                            jQuery('#modal-2').modal('show')
                            $("#text").html("Invalid Address:  " + Address.addressType + ". Please Enter Correct Address");
                            // alert("Invalid Address");
                           // $confirm.modal('hide');
                        }
                    }
                     isaddExistsInMapAlready = false;
                });

            }
        }

        
            if (addressBuilder1 != "") {
                // if (isCustomerDeliverTypeSelected && !isCusDelivTypeUsed) {
                // alert("entered warehouse");
                $.getJSON('https://maps.googleapis.com/maps/api/geocode/json?latlng=' + addressBuilder1 + '&sensor=false&libraries=places&language=' + lang + '', function (data) {
                    // When no data fetched and the Result is Zero it's throwing exception
                    // thats the reason if condition added.
                    if (data.results.length > 0) {

                        isCusDelivTypeUsed = true;
                        
                        var pCode = "Customer Site" + "-" + $('#hpostcode').val();
                        DeleteMarker($('#hpostcode').val(), "Customer Site")
                        for (var i = 0; i < markers.length; i++) {
                            if (markers[i].postcode == pCode) {
                                isaddExistsInMapAlready = true;
                                $confirm.modal('show');

                            }
                        }
                        if (!isaddExistsInMapAlready) {
                            $('#siteaddress').val(addressBuilder1);
                            var p = data.results[0].geometry.location;
                            $('#latitude').text(p.lat);
                            $('#longitude').text(p.lng);



                            var latval;
                            var longval;

                            latval = p.lat * 1000000;
                            latval = Math.round(latval);


                            longval = p.lng * 1000000;
                            longval = Math.round(longval);

                            //document.getElementById("latitude").value = latval;
                            //document.getElementById("longitude").value = longval;

                            var latlng = new google.maps.LatLng(p.lat, p.lng);


                            var Shippingaddress = new google.maps.Marker({
                                position: latlng,
                                icon: SiteAddressImage,//Change made by ankur
                                map: map
                            });



                            // ADD THE POST CODE AND TYPE FROM SIEBEL
                            //$('#hstreet').val() != "" && $('#hcity').val() != "" && $('#hcountry').val() != "" && $('#hpostcode').val() != ""
                            addMarker(Shippingaddress, $('#hpostcode').val(), "Customer Site");

                            // Push your newly created marker into the array:
                            var data1 = "Customer Site" + "-" + $('#hpostcode').val();

                            //addMarker(Shippingaddress, Address.postcode, Address.addressType);

                            // Push your newly created marker into the array:

                            // Push your newly created marker into the array:
                            // var data1 = Address.addressType + "-" + Address.postcode;
                            Shippingaddress.postcode = data1;

                           // gmarkers.push(Shippingaddress);

                            var position = new google.maps.LatLng(p.lat)
                            map.panTo(Shippingaddress.getPosition());
                            map.setCenter(Shippingaddress.getPosition(), 10);

                            //var infowindow =   new google.maps.InfoWindow({
                            //    content: Address.addressType + ":  " + data.results[0].formatted_address
                            //})
                            var infowindow = new google.maps.InfoWindow();
                            infowindow.setContent("Customer Site :  " + data.results[0].formatted_address);
                            //  infowindow.position(position);
                            infowindow.open(map, Shippingaddress);


                            // Changes added by sudhir to show the marker first time
                            //setTimeout(
                            //       function () {
                            //           infowindow.close();
                            //           infowindow.open(map, Shippingaddress);
                            //       }, 30);

                            //resizeMap();
                            //$('#myModal').on('show.bs.modal', function () {
                            //    resizeMap();
                            //})
                            // $confirm.modal('show');


                            // google.maps.event.trigger(map, 'resize');
                            //$confirm.modal('hide');


                            $confirm.modal('show');

                        }
                    }
                    else {
                        // when the map unable to fetch address details 
                        // handling this by showing the pop up message as Invalid address.
                        if (data.status == "ZERO_RESULTS") {
                            // $invAddDlg.html("Invalid Address");
                            jQuery('#modal-2').modal('show')
                            $("#text").html("Invalid Address:  " + Address.addressType + ".");
                            //$invAddDlg.modal('show');
                            // $confirm.modal('hide');
                        }

                        //alert("Invalid Address");
                    }
                    isaddExistsInMapAlready = false;

                });
            }
        }
          
});




function updateStatus() {
    var length = addr1[addr1.length - 1].length;
    if (length == 0) {
        DisablePagging();
        $("#countdata").text("");
    }
    else

        $("#countdata").text(totalrec);
};

var specialKeys = new Array();
specialKeys.push(8); //Backspace
function IsNumeric(e) {
    var keyCode = e.which ? e.which : e.keyCode
    var ret = ((keyCode >= 48 && keyCode <= 57) || specialKeys.indexOf(keyCode) != -1);
    return ret;
}



function isNotZipCode(zip) {
    /// <summary>Validates Zip Code.</summary>
    /// <param name="zip" type="string"></param>
    if ((zip.length != 0) && (zip.length < 4 || zip.length > 7))
        return true;
    else if (isNaN(zip))
        return true;
    return false;
}

function isEmpty(val) {
    /// <summary>Checks whether input control has value or not.</summary>
    /// <param name="" type=""></param>
    if (val.length == 0) return true;
    return false;
}

function ValidateError(element) {
    if (element.id == "city") {
        $("#cityS").hide();
        $("#cityE").show();
        name = jQuery("#city").val();
        char = /[`~!@#$%^&*()_|+\-=?;:'",.<>\{\}\[\]\\\/0-9]/;

        //  jQuery("#city").val(name);
        if ($('#mySelect :selected').val() == "KOREA, REPUBLIC OF") {


            $('#city').css({
                "border": "",
                "background": ""
            });
            $("#cityS").show();
            $("#cityE").hide();


        }

        else {

            //if (element.value.trim() == "" || !isNaN(element.value)) {
                if (element.value.trim() == "" || char.test(name) == true) {
                    $("#cityE").show();
                    $("#cityS").hide();
                    $('#city').css({
                        "border": "1px solid red",
                        "background": "#FFCECE"
                    });
                }

                else {
                    $("#cityE").hide();
                    $("#cityS").show();
                    $('#city').css({
                        "border": "",
                        "background": ""
                    });
                    $("#cityE").hide();
                    $("#cityS").show();
                }
            }

        }
        if (element.id == "address") {
            $("#addressS").hide();
            $("#addressE").show();
            if (element.value.trim() == "") {
                $('#address').css({
                    "border": "1px solid red",
                    "background": "#FFCECE"
                });
            }
            else {
                $('#address').css({
                    "border": "",
                    "background": ""
                });
                $("#addressS").show();
                $("#addressE").hide();
            }

        }
        if (element.id == "postcode") {
            //
            if ($('#mySelect :selected').val() == "KOREA, REPUBLIC OF") {

                if (isNaN(element.value)) {
                    $('#postcode').css({
                        "border": "1px solid red",
                        "background": "#FFCECE"
                    });
                    $("#postcodeE").show();
                    $("#postcodeS").hide();
                }
                else {
                    $('#postcode').css({
                        "border": "",
                        "background": ""
                    });
                    $("#postcodeS").show();
                    $("#postcodeE").hide();
                }
            
            }
            else {

                if (element.value.trim() == "" || isNaN(element.value) || isNotZipCode(element.value)) {
                    $('#postcode').css({
                        "border": "1px solid red",
                        "background": "#FFCECE"
                    });
                    $("#postcodeE").show();
                    $("#postcodeS").hide();
                }
                else {
                    $('#postcode').css({
                        "border": "",
                        "background": ""
                    });
                    $("#postcodeS").show();
                    $("#postcodeE").hide();
                }
            }


        }
    }

    function ValidateSucess(element) {
        if (element.value.length != 0) {
            name = jQuery("#city").val();
            //name = name.replace(/[`~!@#$%^&*()_|+\-=?;:'",.<>\{\}\[\]\\\/0-9]+/g, '');
            //jQuery("#city").val(name);
            name = jQuery("#city").val();
            char = /[`~!@#$%^&*()_|+\-=?;:'",.<>\{\}\[\]\\\/0-9]/;


            if (element.id == "city") {
                if ($('#mySelect :selected').val() == "KOREA, REPUBLIC OF") {


                    $('#city').css({
                        "border": "",
                        "background": ""
                    });
                    $("#cityS").show();
                    $("#cityE").hide();


                }

                else {

                    //if (element.value.trim() == "" || !isNaN(element.value)) {
              
                    if (element.value.trim() == "" || char.test(name) == true) {
                        $("#cityE").show();
                        $("#cityS").hide();
                        $('#city').css({
                            "border": "1px solid red",
                            "background": "#FFCECE"
                        });
                    }

                    else {
                        $("#cityE").hide();
                        $("#cityS").show();
                        $('#city').css({
                            "border": "",
                            "background": ""
                        });
                        $("#cityE").hide();
                        $("#cityS").show();
                    }
                }
            }
            if (element.id == "address") {
                $("#addressS").hide();
                $("#addressE").show();
                if (element.value.trim() == "") {
                    $('#address').css({
                        "border": "1px solid red",
                        "background": "#FFCECE"
                    });
                }
                else {
                    $('#address').css({
                        "border": "",
                        "background": ""
                    });
                    $("#addressS").show();
                    $("#addressE").hide();
                }

            }
            if (element.id == "postcode") {
                //
                if ($('#mySelect :selected').val() == "KOREA, REPUBLIC OF") {
                    if (isNaN(element.value)) {
                        $('#postcode').css({
                            "border": "1px solid red",
                            "background": "#FFCECE"
                        });
                        $("#postcodeE").show();
                        $("#postcodeS").hide();
                    }
                    else {
                        $('#postcode').css({
                            "border": "",
                            "background": ""
                        });
                        $("#postcodeS").show();
                        $("#postcodeE").hide();
                    }

                }
                else {

                    if (element.value.trim() == "" || isNaN(element.value) || isNotZipCode(element.value)) {
                        $('#postcode').css({
                            "border": "1px solid red",
                            "background": "#FFCECE"
                        });
                        $("#postcodeE").show();
                        $("#postcodeS").hide();
                    }
                    else {
                        $('#postcode').css({
                            "border": "",
                            "background": ""
                        });
                        $("#postcodeS").show();
                        $("#postcodeE").hide();
                    }
                }


            }
        }
    }

    function AddRecordInit() {
        $("#checkpartstatus").hide();
        $("#deliverytype").val("");
        $("#address").val("");
        $("#city").val("");
        $("#postcode").val("");
        $("#mySelect").val("");
        $('#txtPartComments').val("");

        $('#deliverytype').removeAttr('disabled');
        $('#address').removeAttr('disabled');
        $('#city').removeAttr('disabled');
        $('#postcode').removeAttr('disabled');
        $('#mySelect').removeAttr('disabled');
        $('#txtPartComments').removeAttr('disabled');
        $('#checkaddress').removeAttr('disabled');
        $("#countdata").text("Creation");
        $('#txtDelivery').removeAttr('disabled');
        $('#imgdel').removeAttr('disabled');

    };

    function InitOldAddress() {

        if (addr1[addr1.length - 1].length != 0) {
            //populateold address
            curenrRecord = 0;
            countdata = addr1[addr1.length - 1].length;

            try {

                var deliverytype = addr1[0][0][0];
                var address = addr1[0][0][1];
                var city = addr1[0][0][2];
                var postcode = addr1[0][0][3];
                var mySelect = addr1[0][0][4];
                var partComment = addr1[0][0][5];
                var deliveryDate = addr1[0][0][6];

                //Status coulumn added by Phani Kanth P.
                var status = addr1[0][0][10];

                $("#deliverytype").val(deliverytype);

                $("#address").val(address);

                $("#city").val(city);

                $("#postcode").val(postcode);

                $("#mySelect").val(mySelect);
                $("#txtPartComments").val(partComment);
                $("#txtDelivery").val(deliveryDate);

                curenrRecord = curenrRecord + 1;

                totalrec = curenrRecord + "/" + countdata;

                if ($('#deliverytype').val() != "Customer Site") {

                    if ($('#checkStatval').val() != "Cancelled") {

                        $('#deliverytype').removeAttr('disabled');
                        $('#address').removeAttr('disabled');
                        $('#city').removeAttr('disabled');
                        $('#postcode').removeAttr('disabled');
                        $('#mySelect').removeAttr('disabled');
                        $('#txtPartComments').removeAttr('disabled');
                        $('#checkaddress').removeAttr('disabled');
                        $("#countdata").text("Creation");
                        $('#txtDelivery').removeAttr('disabled');

                        //Modify and cancelled status added by phani kanth
                        $("#checkpartstatus").hide();
                        $('#imgdel').removeAttr('disabled');

                    }



                }
                else {
                    $('#deliverytype').attr("disabled", "disabled");
                    $('#address').attr("disabled", "disabled");
                    $('#city').attr("disabled", "disabled");
                    $('#postcode').attr("disabled", "disabled");
                    $('#txtPartComments').removeAttr('disabled');
                    $('#mySelect').attr("disabled", "disabled");
                    $('#checkaddress').attr("disabled", "disabled");
                    $('#txtDelivery').removeAttr('disabled');

                    //Modify and cancelled status added by phani kanth
                    $("#checkpartstatus").hide();
                    $('#imgdel').removeAttr('disabled');

                }
                //Status completed condition added by Phani Kanth P.
                if (status == "Assigned" || status == "Acknowledged" || status == "En Route" || status == "On Site" || status == "Completed" || status == "Incomplete" || status == "Cancelled" || status == "Rejected") {
                    // if (status == "Completed") {
                    $('#deliverytype').attr("disabled", "disabled");
                    $('#address').attr("disabled", "disabled");
                    $('#city').attr("disabled", "disabled");
                    $('#postcode').attr("disabled", "disabled");
                    $('#txtPartComments').attr("disabled", "disabled");
                    $('#mySelect').attr("disabled", "disabled");
                    $('#checkaddress').attr("disabled", "disabled");
                    $('#txtDelivery').attr("disabled", "disabled");

                    //Modify and cancelled status added by phani kanth
                    $('#imgdel').attr("disabled", "disabled");
                    $("#checkpartstatus").show();
                    $("#checkpartstatus").text(status + " " + "Part pickup job cannot be modified/cancelled.");

                }
                var mainStatus = $('#checkStatval').val();
                if (mainStatus == "Assigned" || mainStatus == "Acknowledged" || mainStatus == "En Route" || mainStatus == "On Site" || mainStatus == "Completed" || mainStatus == "Incomplete" || mainStatus == "Cancelled" || mainStatus == "Rejected") {

                    // if ($('#checkStatval').val() == "Cancelled") {
                    $('#imgplus').attr("disabled", "disabled");
                    $('#imgdel').attr("disabled", "disabled");
                    $('#btnYesConfirmYesNo').attr("disabled", "disabled");
                    $('#imgPrevious').attr("disabled", "disabled");
                    $('#imgnext').attr("disabled", "disabled");
                    $('#checkaddress').attr("disabled", "disabled");

                    //Modify and cancelled status added by phani kanth
                    //  $("#checkpartstatus").hide();
                    //  $('#imgdel').removeAttr('disabled');
                   // $("#checkpartstatus").show();
                   // $("#checkpartstatus").text(mainStatus + " " + "Part pickup job cannot be modified/cancelled.");

                }
            }
            catch (ex) {
            }// $("#countdata").val(totalrec);}



            updateStatus();
        }

    }

    function populateOldAddress(deliverytype, address, city, postcode, countryData, partComment, deliveryDate, latitude, longitude, number, status, isMST, isCritical) {
        // if (deliverytype == "Customer Site") {
        $('#latitude').val(latitude);
        $('#longitude').val(longitude);
        // }
        // populateOldAddress
        if (deliverytype == "Customer Site") {
            //No need to replace, code Removed
            // address = address.replace(/\,/, " ").trim();
            //Changes done on 9/6/2016
            //partComment = partComment.replace(/,/g, "");
        }

        if (deliverytype != "") {

            if (countryData == "KOREA REPUBLIC OF")
                countryData = "KOREA, REPUBLIC OF";
            if (status == "") {
                addr1[addr1.length - 1].push([deliverytype, address, city, postcode, countryData, partComment, deliveryDate, latitude, longitude]);

                addr2[addr2.length - 1].push([deliverytype, address, city, postcode, countryData, partComment, deliveryDate, latitude, longitude]);

            }
            else {
                addr1[addr1.length - 1].push([deliverytype, address, city, postcode, countryData, partComment, deliveryDate, latitude, longitude, number, status, isMST, isCritical]);

                addr2[addr2.length - 1].push([deliverytype, address, city, postcode, countryData, partComment, deliveryDate, latitude, longitude, number, status, isMST, isCritical]);

            }

        }

        // below code is when we click on modify visit customer address is not showing.
        if (deliverytype == "Customer Site") {
            // Below line commented as it is showing 2 custom addresses on Map.
            // isCustomerDeliverTypeSelected = true;
            $('#latitude').val($('#hlatitude').val());
            $('#longitude').val($('#hlongitude').val());
            //addressBuilder1 = address + " " + city + " " + postcode + " " + countryData;

            addressBuilder1 = $('#hlatitude').val() + ", " + $('#hlongitude').val();
        }
    }


    function AddRecord() {
        var deliverytype = $('#deliverytype :selected').text();
        var countryData = $('#mySelect :selected').text();

        var address = $("#address").val();
        address = address.trim();
        if (deliverytype == "Customer Site") {
            $('#latitude').val($('#hlatitude').val());
            $('#longitude').val($('#hlongitude').val());
            countryData = $('#hcountry').val();
            //No need to replace, code Removed
            //if (countryData.indexOf(',') != -1) {
            //    countryData = countryData.replace(/,/g, "");
            //}

            //if (countryData=="KOREA REPUBLIC OF") {
            //    countryData = "South Korea";
            //}
        }

        var city = $("#city").val();
        city = city.trim();
        var latitude = $('#latitude').val();

        var longitude = $('#longitude').val();

        var partComment = $("#txtPartComments").val();

        var deliveryDate = $("#txtDelivery").val();

        var postcode = $("#postcode").val().trim();
        //Changes done by raju on 9/6/2016

        partComment = partComment.trim();


        //if (city.indexOf(',') != -1) {
        //    city = city.replace(/,/g, "");

        //}
        //if (address.indexOf(',') != -1) {
        //    address = address.replace(/,/g, "");

        //}
        //if (countryData.indexOf(',') != -1) {
        //    countryData = countryData.replace(/,/g, "");

        //}
        //if (partComment.indexOf(',') != -1) {
        //    partComment = partComment.replace(/,/g, "");

        //}
        //if (address.indexOf('=') != -1) {
        //    address = address.replace(/=/g, "");
        //}
        //if (partComment.indexOf('=') != -1) {
        //    partComment = partComment.replace(/=/g, "");

        //}
        //if (city.indexOf('=') != -1) {
        //    city = city.replace(/=/g, "");

        //}



        //partComment = partComment.replace(/=/g, "").replace(/,/g," ");

        //debugger;


        if (countdata > 0) {
            while (curenrRecord > 0) {

                if (deliverytype == addr1[0][curenrRecord - 1][0] && address == addr1[0][curenrRecord - 1][1] && city == addr1[0][curenrRecord - 1][2] && postcode == addr1[0][curenrRecord - 1][3] && countryData == addr1[0][curenrRecord - 1][4]) {
                    if (addr1[0][curenrRecord - 1].length > 10) {
                        if (addr1[0][curenrRecord - 1][10] != "Cancelled") {
                            var $confirmAdd = $("#DuplicateAddresss");
                            $confirmAdd.modal('show');
                            return false;
                        }
                    }

                    else {
                        var $confirmAdd = $("#DuplicateAddresss");
                        $confirmAdd.modal('show');
                        return false;
                    }
                }

                curenrRecord--;
            }
            if (deliverytype != "") {


                AddorUpdatePartAddress(deliverytype, address, city, postcode, countryData, partComment, deliveryDate, latitude, longitude);

                $('#arrayAddress').text(addr1);
                countdata = addr1[addr1.length - 1].length;
                curenrRecord = addr1[addr1.length - 1].length;
                totalrec = curenrRecord + "/" + countdata;
                updateStatus();
                EnablePagging();
            }
        }
        else {
            if (deliverytype != "") {

                AddorUpdatePartAddress(deliverytype, address, city, postcode, countryData, partComment, deliveryDate, latitude, longitude);

                $('#arrayAddress').text(addr1);
                countdata = addr1[addr1.length - 1].length;
                curenrRecord = addr1[addr1.length - 1].length;
                totalrec = curenrRecord + "/" + countdata;
                updateStatus();
                EnablePagging();
            }
        }
    }

    function DeleteAllmarkers() {

        markers = [];

        for (var i = 0; i < markers.length; i++) {

            markers[i].setMap(null);

        }
    }

    function AddOrUpdateComment(deliverytype, partComment, deliveryDate, address) {
        //  var deliverytypeold
        //Changes done on 9/6/2016
        address = address.trim();
        partComment = partComment.trim();
        //No need to replace, code Removed
        //if (address.indexOf(',') != -1) {
        //    address = address.replace(/,/g, "");

        //}

        //if (partComment.indexOf(',') != -1) {
        //    partComment = partComment.replace(/,/g, "");

        //}
        //if (address.indexOf('=') != -1) {
        //    address = address.replace(/=/g, "");
        //}
        //if (partComment.indexOf('=') != -1) {
        //    partComment = partComment.replace(/=/g, "");

        //}



        for (var i = 0; i < addr1[addr1.length - 1].length; i++) {

            if (addr1[0][i][0] == deliverytype && addr1[0][i][1] == address) {

                //Add Or Update Comment

                addr1[0][i][5] = partComment;
                addr1[0][i][6] = deliveryDate;


            }

        }

    }
    function AddOrUpdateComment2(deliverytype, partComment, deliveryDate, address) {
        //  var deliverytypeold
        //Changes done on 9/6/2016
        address = address.trim();
        partComment = partComment.trim();
        //No need to replace, code Removed
        //address = address.replace(/=/g, "");
        //partComment = partComment.replace(/=/g, "");
        //if (address.indexOf(',') != -1) {
        //    address = address.replace(/,/g, "");
        // }

        //if (partComment.indexOf(',') != -1) {
        //    partComment = partComment.replace(/,/g, "");

        //}
        //if (address.indexOf('=') != -1) {
        //    address = address.replace(/=/g, "");
        //}
        //if (partComment.indexOf('=') != -1) {
        //    partComment = partComment.replace(/=/g, "");

        //}


        for (var i = 0; i < addr2[addr2.length - 1].length; i++) {

            if (addr2[0][i][0] == deliverytype && addr2[0][i][1] == address) {

                //Add Or Update Comment

                addr2[0][i][5] = partComment;
                addr2[0][i][6] = deliveryDate;
            }

        }

    }
    
    function AddorUpdatePartAddress(deliverytype, address, city, postcode, countryData, partComment, deliveryDate, latitude, longitude) {
        var statusChange = "";

        //  var deliverytypeold
        if (address != "") {
            for (var i = 0; i < addr1[addr1.length - 1].length; i++) {

                if (addr1[0][i].length > 10 && addr1[0][i][0] == deliverytype) {
                    if (addr1[0][i][0] == deliverytype && (addr1[0][i][10] == "New" || addr2[0][i][10] == "Tentative")) {

                        //deleteold marker
                        DeleteMarker(addr1[0][i][3], addr1[0][i][0]);

                        addr1[0].splice(i, 1);

                    }
                }
                else if (addr1[0][i][0] == deliverytype && addr1[0][i].length == 9) {
                    //deleteold marker
                    DeleteMarker(addr1[0][i][3], addr1[0][i][0]);

                    addr1[0].splice(i, 1);

                }
            }

            addr1[addr1.length - 1].push([deliverytype, address, city, postcode, countryData, partComment, deliveryDate, latitude, longitude]);
            for (var j = 0; j < addr2[addr2.length - 1].length; j++) {

                if (addr2[0][j][0] == deliverytype) {
                    //deleteold marker
                    if (addr2[0][j].length > 10) {
                        //status = addr2[0][j][9];
                        if (addr2[0][j][10] == "New" || addr2[0][j][10] == "Tentative") {
                            statusChange = addr2[0][j][10];
                            addr2[0][j][0] = deliverytype;
                            addr2[0][j][1] = address;
                            addr2[0][j][2] = city;
                            addr2[0][j][3] = postcode;
                            addr2[0][j][4] = countryData;
                            addr2[0][j][5] = partComment;
                            addr2[0][j][6] = deliveryDate;
                            addr2[0][j][7] = latitude;
                            addr2[0][j][8] = longitude;
                        }
                    }
                    else if (addr2[0][j].length == 9) {
                        addr2[0].splice(j, 1);
                    }
                }

            }
            if (statusChange == "") {
                addr2[addr2.length - 1].push([deliverytype, address, city, postcode, countryData, partComment, deliveryDate, latitude, longitude]);
            }
        }
    }


    function EnablePagging() {
        $('#imgdel').attr('src', '/Images/minus.png');
        $('#imgplus').attr('src', '/Images/plus.png');
        $('#imgPrevious').attr('src', '/Images/previous.png');
        $('#imgnext').attr('src', '/Images/next.png');
    }
    function DisablePagging() {
        $('#imgdel').attr('src', '/Images/minus-disable.png');
        $('#imgplus').attr('src', '/Images/plus.png');
        $('#imgPrevious').attr('src', '/Images/previous-disable.png');
        $('#imgnext').attr('src', '/Images/next-disable.png');
    }

    var countData2 = 0;
    function DeleteRow() {

        if (countdata > 0) {

            if (countdata == 1) {
                DeleteMarker(addr1[0][curenrRecord - 1][3], addr1[0][curenrRecord - 1][0]);
                //if (addr2[0][addr2.length - 1][9] != null || addr2[0][addr2.length - 1][9] !="")
                //{
                //    addr2[0][curenrRecord - 1][9] = "Cancelled";
                //}
                //Set the Cancel Status to part tool details delete from UI
                for (i = 0; i < addr2[addr2.length - 1].length; i++) {

                    if (addr2[0][i][3] == addr1[0][curenrRecord - 1][3] && addr2[0][i][2] == addr1[0][curenrRecord - 1][2] && addr2[0][i][1] == addr1[0][curenrRecord - 1][1] && addr2[0][i][0] == addr1[0][curenrRecord - 1][0]) {

                        if (addr2[0][i].length > 10) {
                            addr2[0][i][10] = "Cancelled";
                        }

                        else {
                            addr2[0].splice(i, 1);
                        }
                    }


                }
                addr1[addr1.length - 1].pop();
                $('#arrayAddress').text(addr1);
                countdata = addr1[addr1.length - 1].length;

                $("#deliverytype").val("");
                $("#address").val("");
                $("#city").val("");
                $("#postcode").val("");
                $("#mySelect").val("");
                $('#txtPartComments').val("");
                $('#txtDelivery').val("");
                curenrRecord = curenrRecord - 1;
                totalrec = curenrRecord + "/" + countdata;

            } else {
                if (curenrRecord >= 2) {


                    DeleteMarker(addr1[0][curenrRecord - 1][3], addr1[0][curenrRecord - 1][0]);
                    //if (addr2[0][curenrRecord - 1][9] != null || addr2[0][curenrRecord - 1][9] !="")
                    //{
                    //    addr2[0][curenrRecord - 1][9] = "Cancelled";
                    //}
                    //Set the Cancel Status to part tool details delete from UI
                    for (i = 0; i < addr2[addr2.length - 1].length; i++) {
                        if (addr2[0][i][3] == addr1[0][curenrRecord - 1][3] && addr2[0][i][2] == addr1[0][curenrRecord - 1][2] && addr2[0][i][1] == addr1[0][curenrRecord - 1][1] && addr2[0][i][0] == addr1[0][curenrRecord - 1][0]) {

                            if (addr2[0][i].length > 10) {
                                addr2[0][i][10] = "Cancelled";
                            }

                            else {
                                addr2[0].splice(i, 1);
                            }
                        }

                    }

                    //for (j=0;j < partAddress[partAddress.length-1].length;j++)
                    //{
                    //    if (addr2[0][i][3] == addr1[0][curenrRecord - 1][3] && addr2[0][i][0] == addr1[0][curenrRecord - 1][0])
                    //    {
                    //        if (addr2[0][i][3] == addr1[0][curenrRecord - 1][3] && addr2[0][i][0] == addr1[0][curenrRecord - 1][0])
                    //            addr2[0][i][9] = "Cancelled";
                    //    }

                    addr1[0].splice(curenrRecord - 1, 1);
                    var deliverytype = addr1[0][curenrRecord - 2][0];
                    var address = $("#address").val();
                    var city = $("#city").val();
                    var postcode = $("#postcode").val();
                    var countryData = addr1[0][curenrRecord - 2][4];


                    countdata = addr1[addr1.length - 1].length;
                    countData2 = addr2[addr2.length - 1].length;
                    $("#deliverytype").val(addr1[0][curenrRecord - 2][0]);
                    $("#address").val((addr1[0][curenrRecord - 2][1]));
                    $("#city").val((addr1[0][curenrRecord - 2][2]));
                    $("#postcode").val((addr1[0][curenrRecord - 2][3]));
                    $("#mySelect").val(addr1[0][curenrRecord - 2][4]);
                    $("#txtPartComments").val(addr1[0][curenrRecord - 2][5]);
                    $("#txtDelivery").val(addr1[0][curenrRecord - 2][6]);
                    if (addr1[0][curenrRecord - 2].length > 10)
                        deliveryTypeStatus = addr1[0][curenrRecord - 2][10];

                    curenrRecord = curenrRecord - 1;
                    totalrec = curenrRecord + "/" + countdata;
                }
                else {

                    if (curenrRecord == 1) {
                        DeleteMarker(addr1[0][curenrRecord - 1][3], addr1[0][curenrRecord - 1][0]);
                        //if (addr2[0][curenrRecord - 1][9] != null || addr2[0][curenrRecord - 1][9] !="")
                        //{
                        //    addr2[0][curenrRecord - 1][9] = "Cancelled";
                        //}
                        //Set the Cancel Status to part tool details delete from UI
                        for (i = 0; i < addr2[addr2.length - 1].length; i++) {
                            if (addr2[0][i][3] == addr1[0][curenrRecord - 1][3] && addr2[0][i][2] == addr1[0][curenrRecord - 1][2] && addr2[0][i][1] == addr1[0][curenrRecord - 1][1] && addr2[0][i][0] == addr1[0][curenrRecord - 1][0]) {

                                if (addr2[0][i].length > 10) {
                                    addr2[0][i][10] = "Cancelled";
                                }

                                else {
                                    addr2[0].splice(i, 1);
                                }
                            }
                        }
                        addr1[0].splice(curenrRecord - 1, 1);

                    }
                    else {
                        // addr1[0].splice(curenrRecord - 1, 1);
                    }


                    var deliverytype = addr1[0][curenrRecord - 1][0];
                    var address = $("#address").val();
                    var city = $("#city").val();
                    var postcode = $("#postcode").val();
                    var countryData1 = addr1[0][curenrRecord - 1][4];
                    countdata = addr1[addr1.length - 1].length;
                    $("#deliverytype").val(addr1[0][curenrRecord - 1][0]);
                    $("#address").val((addr1[0][curenrRecord - 1][1]));
                    $("#city").val((addr1[0][curenrRecord - 1][2]));
                    $("#postcode").val((addr1[0][curenrRecord - 1][3]));
                    $("#mySelect").val(addr1[0][curenrRecord - 1][4]);
                    $("#txtPartComments").val(addr1[0][curenrRecord - 1][5]);
                    $("#txtDelivery").val(addr1[0][curenrRecord - 1][6]);
                    if (addr1[0][curenrRecord - 1].length > 10)
                        deliveryTypeStatus = addr1[0][curenrRecord - 1][10];

                    curenrRecord = curenrRecord;
                    totalrec = curenrRecord + "/" + countdata;
                }
            }
        }
        updateStatus();
        disableControls();
    }

    function nextdata() {
        $("#checkpartcheckclick").hide();
        var x = document.getElementById("mySelect")
        if (curenrRecord >= 1 && curenrRecord < countdata) {
            try {
                $("#deliverytype").val(addr1[0][curenrRecord][0]);

                $("#address").val((addr1[0][curenrRecord][1]));

                $("#city").val((addr1[0][curenrRecord][2]));

                $("#postcode").val((addr1[0][curenrRecord][3]));

                if (addr1[0][curenrRecord][4] == "KOREA REPUBLIC OF") {
                    addr1[0][curenrRecord][4] = "KOREA, REPUBLIC OF";
                }

                $("#mySelect").val(addr1[0][curenrRecord][4]);
                $("#txtPartComments").val(addr1[0][curenrRecord][5]);
                $("#txtDelivery").val(addr1[0][curenrRecord][6]);
                //Status coulumn added by Phani Kanth P.
                var status = addr1[0][curenrRecord][10]

                curenrRecord = curenrRecord + 1;

                totalrec = curenrRecord + "/" + countdata;


                if ($('#deliverytype').val() != "Customer Site") {

                    if ($('#checkStatval').val() != "Cancelled") {
                        $('#deliverytype').removeAttr('disabled');
                        $('#address').removeAttr('disabled');
                        $('#city').removeAttr('disabled');
                        $('#postcode').removeAttr('disabled');
                        $('#mySelect').removeAttr('disabled');
                        $('#txtPartComments').removeAttr('disabled');
                        $('#checkaddress').removeAttr('disabled');
                        $("#countdata").text("Creation");
                        $('#txtDelivery').removeAttr('disabled');

                        //Modify and cancelled status added by phani kanth
                        $('#imgdel').removeAttr('disabled');
                        $("#checkpartstatus").hide();
                    }
                    //Status completed condition added by Phani Kanth P.

                }
                else {
                    $('#deliverytype').attr("disabled", "disabled");
                    $('#address').attr("disabled", "disabled");
                    $('#city').attr("disabled", "disabled");
                    $('#postcode').attr("disabled", "disabled");
                    $('#txtPartComments').removeAttr("disabled", "disabled");
                    $('#mySelect').attr("disabled", "disabled");
                    $('#checkaddress').attr("disabled", "disabled");
                    $('#txtDelivery').removeAttr("disabled", "disabled");

                    //Modify and cancelled status added by phani kanth
                    $("#checkpartstatus").hide();
                    $('#imgdel').removeAttr("disabled", "disabled");
                }
                if (status == "Assigned" || status == "Acknowledged" || status == "En Route" || status == "On Site" || status == "Completed" || status == "Incomplete" || status == "Cancelled" || status == "Rejected") {
                    $('#deliverytype').attr("disabled", "disabled");
                    $('#address').attr("disabled", "disabled");
                    $('#city').attr("disabled", "disabled");
                    $('#postcode').attr("disabled", "disabled");
                    $('#txtPartComments').attr("disabled", "disabled");
                    $('#mySelect').attr("disabled", "disabled");
                    $('#checkaddress').attr("disabled", "disabled");
                    $('#txtDelivery').attr("disabled", "disabled");

                    //Modify and cancelled status added by phani kanth
                    $("#checkpartstatus").show();
                    $("#checkpartstatus").text(status + " " + "Part pickup job cannot be modified/cancelled.");
                    $('#imgdel').attr("disabled", "disabled");
                }
            }
            catch (ex) {
            }// $("#countdata").val(totalrec);}

        }

        updateStatus();

    }



    function prevdata() {
        $("#checkpartcheckclick").hide();
        var y = document.getElementById("mySelect")
        var status = "";
        if (curenrRecord >= 1) {

            if (curenrRecord > 1) {
                $("#deliverytype").val(addr1[0][curenrRecord - 2][0]);


                $("#address").val((addr1[0][curenrRecord - 2][1]));
                $("#city").val((addr1[0][curenrRecord - 2][2]));
                $("#postcode").val((addr1[0][curenrRecord - 2][3]));

                if (addr1[0][curenrRecord - 2][4] == "KOREA REPUBLIC OF") {
                    addr1[0][curenrRecord - 2][4] = "KOREA, REPUBLIC OF";
                }


                $("#mySelect").val(addr1[0][curenrRecord - 2][4]);
                $("#txtPartComments").val(addr1[0][curenrRecord - 2][5]);
                $("#txtDelivery").val(addr1[0][curenrRecord - 2][6]);
                //Status coulumn added by Phani Kanth P.
                status = addr1[0][curenrRecord - 2][10];
                curenrRecord = curenrRecord - 1;
                totalrec = curenrRecord + "/" + countdata;
            }
            else {
                $("#deliverytype").val(addr1[0][curenrRecord - 1][0]);


                $("#address").val((addr1[0][curenrRecord - 1][1]));
                $("#city").val((addr1[0][curenrRecord - 1][2]));
                $("#postcode").val((addr1[0][curenrRecord - 1][3]));

                if (addr1[0][curenrRecord - 1][4] == "KOREA REPUBLIC OF") {
                    addr1[0][curenrRecord - 1][4] = "KOREA, REPUBLIC OF";
                }


                $("#mySelect").val(addr1[0][curenrRecord - 1][4]);
                $("#txtPartComments").val(addr1[0][curenrRecord - 1][5]);
                $("#txtDelivery").val(addr1[0][curenrRecord - 1][6]);
                //Status coulumn added by Phani Kanth P.
                status = addr1[0][curenrRecord - 1][10];
                //curenrRecord = curenrRecord - 1;
                totalrec = curenrRecord + "/" + countdata;
            }



            if ($('#deliverytype').val() != "Customer Site") {


                if ($('#checkStatval').val() != "Cancelled") {
                    $('#deliverytype').removeAttr('disabled');
                    $('#address').removeAttr('disabled');
                    $('#city').removeAttr('disabled');
                    $('#postcode').removeAttr('disabled');
                    $('#mySelect').removeAttr('disabled');
                    $('#txtPartComments').removeAttr('disabled');
                    $('#checkaddress').removeAttr('disabled');
                    $("#countdata").text("Creation");
                    $('#txtDelivery').removeAttr('disabled');

                    //Modify and cancelled status added by phani kanth
                    $("#checkpartstatus").hide();
                    $('#imgdel').removeAttr('disabled');
                }

            }
            else {
                $('#deliverytype').attr("disabled", "disabled");
                $('#address').attr("disabled", "disabled");
                $('#city').attr("disabled", "disabled");
                $('#postcode').attr("disabled", "disabled");
                $('#txtPartComments').removeAttr("disabled", "disabled");
                $('#mySelect').attr("disabled", "disabled");
                $('#checkaddress').attr("disabled", "disabled");
                $('#txtDelivery').removeAttr("disabled", "disabled");


                //Modify and cancelled status added by phani kanth
                $("#checkpartstatus").hide();
                $('#imgdel').removeAttr("disabled", "disabled");

            }

            //Status completed condition added by Phani Kanth P.
            if (status == "Assigned" || status == "Acknowledged" || status == "En Route" || status == "On Site" || status == "Completed" || status == "Incomplete" || status == "Cancelled" || status == "Rejected") {
                $('#deliverytype').attr("disabled", "disabled");
                $('#address').attr("disabled", "disabled");
                $('#city').attr("disabled", "disabled");
                $('#postcode').attr("disabled", "disabled");
                $('#txtPartComments').attr("disabled", "disabled");
                $('#mySelect').attr("disabled", "disabled");
                $('#checkaddress').attr("disabled", "disabled");
                $('#txtDelivery').attr("disabled", "disabled");

                //Modify and cancelled status added by phani kanth
                $("#checkpartstatus").show();
                $("#checkpartstatus").text(status + " " + "Part pickup job cannot be modified/cancelled.");
                $('#imgdel').attr("disabled", "disabled");
            }
        }

        updateStatus();
    }

    //barnali 
    function AsyncConfirmYesNo() {
        var $confirm = $("#CheckAddresspopup");
        // $confirm.modal('show');
        $("#btnYesConfirmYesNo").off('click').click(function () {
            AddRecord();

            //if ($('#postcode').val() != "" && $('#deliverytype :selected').text() != "" ) {
                DeleteMarker($('#postcode').val(), $('#deliverytype :selected').text());
           // }
            $confirm.modal("hide");
            $("#checkpartcheckclick").show();
            $("#checkpartcheckclick").text($('#deliverytype :selected').text() + " " + "Check address is completed successfully");
            PartDataModified = true;


        });
        $("#btnNoConfirmYesNo").off('click').click(function () {
            $confirm.modal("hide");
            //if ($('#postcode').val() != "" && $('#deliverytype :selected').text() != "" && $("#countdata").text() == "Creation") {
                DeleteMarker($('#postcode').val(), $('#deliverytype :selected').text());
           // }
            $("#checkpartcheckclick").hide();
        });
    }

    function AsyncConfirmGooglemapYesNo() {

        //Added by Barnali 
        // if ($('#address').val() != "" && $('#deliverytype :selected').text() != "" && $('#city').val() != "" && $('#mySelect :selected').text() != "" && $('#postcode').val() != "") {

        if ($('#address').val() != "" && $('#deliverytype :selected').text() != "" && $('#mySelect :selected').text() != "") {
            if (countdata > 0) {

                if ($('#postcode').val() == addr1[0][curenrRecord - 1][3]) {
                    var $confirm = $("#Googlemappopup");
                    $confirm.modal('show');
                    $("#btnYesConfirmGoogleMap").off('click').click(function () {
                        DeleteRow();
                        $confirm.modal("hide");
                    });
                }
                else {
                    if ($('#postcode').val() != "" && $('#deliverytype :selected').text() != "" && $("#countdata").text() == "Creation") {
                        DeleteMarker($('#postcode').val(), $('#deliverytype :selected').text());
                    }
                    clearControls();
                    prevdata();
                }
                $("#btnNoConfirmGoogleMap").off('click').click(function () {
                    $confirm.modal("hide");

                });
            }

            if ($('#deliverytype :selected').text() == "Customer Site" && isCustomerDeliverTypeSelected)
                isCustomerDeliverTypeSelected = false;
        }
    }


    //Code added by Barnali 
    //var markers = [];
    function addMarker(markerad, postcode, deliveryType) {
        var flag = true;
        var data = deliveryType + "-" + postcode;
        if (markers.length > 0) {

            for (var i = 0; i < markers.length; i++) {
                if (markers[i].postcode == data) {
                    flag = false;
                }
            }
        }
        if (flag) {
            var marker = markerad;
            marker.postcode = data;
            markers.push(marker);
        }

    }


    function DeleteMarker(postcode, deliveryType) {



        var data = deliveryType + "-" + postcode;

        for (var i = 0; i < markers.length; i++) {

            if (markers[i].postcode == data) {

                markers[i].setMap(null);

                markers.splice(i, 1);

                // return;
            }
        }

        for (i = 0; i < gmarkers.length; i++) {

            if (gmarkers[i].postcode == data) {

                gmarkers[i].setMap(null);

                gmarkers.splice(i, 1);

                //return;
            }

            //gmarkers[i].setMap(null);
        }
    }


    function clearControls() {
        $("#deliverytype").val('');
        $("#address").val('');
        $("#city").val('');
        $("#postcode").val('');
        $("#mySelect").val('');
        $("#txtDelivery").val('');

        $("#cityE").hide();
        $("#cityS").hide();
        $("#addressE").hide();
        $("#addressS").hide();
        $("#postcodeE").hide();
        $("#postcodeS").hide();
    }

    var deliveryTypeStatus = "";
    function disableControls() {

        var deliverytype = $('#deliverytype :selected').text();

        if (deliverytype != "") {
            if (deliverytype == "Customer Site") {
                $('#deliverytype').attr("disabled", "disabled");
                $('#address').attr("disabled", "disabled");
                $('#city').attr("disabled", "disabled");
                $('#postcode').attr("disabled", "disabled");
                $('#txtPartComments').removeAttr("disabled", "disabled");
                $('#mySelect').attr("disabled", "disabled");
                $('#checkaddress').attr("disabled", "disabled");
                $('#txtDelivery').removeAttr("disabled", "disabled");
            }
            else {

                $('#deliverytype').removeAttr('disabled');
                $('#address').removeAttr('disabled');
                $('#city').removeAttr('disabled');
                $('#postcode').removeAttr('disabled');
                $('#mySelect').removeAttr('disabled');
                $('#txtPartComments').removeAttr('disabled');
                $('#checkaddress').removeAttr('disabled');
                $('#txtDelivery').removeAttr('disabled');
            }

        }
        else {

            $('#deliverytype').attr("disabled", "disabled");
            $('#address').attr("disabled", "disabled");
            $('#city').attr("disabled", "disabled");
            $('#postcode').attr("disabled", "disabled");
            $('#txtPartComments').attr("disabled", "disabled");
            $('#mySelect').attr("disabled", "disabled");
            $('#checkaddress').attr("disabled", "disabled");
            $('#txtDelivery').attr("disabled", "disabled");
        }

        if (deliveryTypeStatus == "Assigned" || deliveryTypeStatus == "Acknowledged" || deliveryTypeStatus == "En Route" || deliveryTypeStatus == "On Site" || deliveryTypeStatus == "Completed" || deliveryTypeStatus == "Incomplete" || deliveryTypeStatus == "Cancelled" || deliveryTypeStatus == "Rejected") {
            $('#deliverytype').attr("disabled", "disabled");
            $('#address').attr("disabled", "disabled");
            $('#city').attr("disabled", "disabled");
            $('#postcode').attr("disabled", "disabled");
            $('#txtPartComments').attr("disabled", "disabled");
            $('#mySelect').attr("disabled", "disabled");
            $('#checkaddress').attr("disabled", "disabled");
            $('#txtDelivery').attr("disabled", "disabled");

            //Modify and cancelled status added by phani kanth
            $("#checkpartstatus").show();
            $("#checkpartstatus").text(deliveryTypeStatus + " " + "Part pickup job cannot be modified/cancelled.");
            $('#imgdel').attr("disabled", "disabled");
        }

    }

    function enableControls() {

        $('#deliverytype').removeAttr('disabled');
        $('#address').removeAttr('disabled');
        $('#city').removeAttr('disabled');
        $('#postcode').removeAttr('disabled');
        $('#mySelect').removeAttr('disabled');
        $('#txtPartComments').removeAttr('disabled');
        $('#checkaddress').removeAttr('disabled');
        $("#countdata").text("Creation");
        $('#txtDelivery').removeAttr('disabled');

    }


    $('#myModal').on('show.bs.modal', function () {
        resizeMap();
    })

    function resizeMap() {
        if (typeof map == "undefined") return;

        var center = map.getCenter();
        google.maps.event.trigger(map, "resize");
        map.setCenter(center);
    }


