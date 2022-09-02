
/// <reference path="G:\Externalmodules\ ShipHawkIntegration\ ShipHawkIntegration\Config.js" />
var baseUrl = "http://localhost/MagentoIntegration";
var companyId = 0;
var docNo = "";
var Id = "";
var voucherType = 0;
var LoginId = 0;
var sessionId = "";
var name = "";
var code = "";
var Accounttype = 0;
var TaxCategory = 0;



function ItemCreate() {
    debugger;
    Focus8WAPI.getGlobalValue("ItemCallback", '*', 1);
}

function ItemCallback(response) {
    debugger;
    companyId = response.data.CompanyId;
    sessionId = response.data.SessionId;
    Focus8WAPI.getFieldValue("ItemDataCallback", ["sName", "sCode","TaxCategory"], 1, false, 111);
}

function ItemDataCallback(response1) {
    debugger;
    name = response1.data[0].FieldValue;
    code = response1.data[1].FieldValue;
    TaxCategory = response1.data[2].FieldValue;
    $.ajax({
        url: baseUrl + "/Magento/ItemData",
        type: "POST",
        data: { companyId: companyId, name: name, code: code, TaxCategory: TaxCategory },
        success: function (result) {
            debugger;
            console.log(result.status)
            console.log(result.message)
            alert(result.message);
            Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.MASTER, true);
        },
        error: function (e) {
            console.log(e);
            debugger;
        }
    });
}

function ItemDelete() {
    debugger;
    console.log("item delete fired .");
    Focus8WAPI.getGlobalValue("ItemCallbackDelete", '*', 1);
}

function ItemCallbackDelete(response) {
    debugger;
    companyId = response.data.CompanyId;
    sessionId = response.data.SessionId;
    Focus8WAPI.getFieldValue("ItemDataCallbackDelete", ["sName", "sCode"], 1, false, 222);
}

function ItemDataCallbackDelete(response1) {
    debugger;
    name = response1.data[0].FieldValue;
    code = response1.data[1].FieldValue;

    $.ajax({
        url: baseUrl + "/Master/ItemDelete",
        type: "POST",
        data: { companyId: companyId, name: name, code: code },
        success: function (result) {
            debugger;
            alert(result.message)
            Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.MASTER, true);

        },
        error: function (e) {
            console.log(e);
            debugger;
        }
    });
}


function CustomerCreate() {
    debugger;
    Focus8WAPI.getGlobalValue("CustomerCallback", '*', 1);
}

function CustomerCallback(response) {
    debugger;
    companyId = response.data.CompanyId;
    sessionId = response.data.SessionId;
    Focus8WAPI.getFieldValue("CustomerDataCallback", ["sName", "sCode","iAccountType"], 1, false, 111);
}

function CustomerDataCallback(response1) {
    debugger;
    name = response1.data[0].FieldValue;
    code = response1.data[1].FieldValue;
    Accounttype = response1.data[1].FieldValue;
    if (Accounttype = 5) {


        $.ajax({
            url: baseUrl + "/Master/CustomerData",
            type: "POST",
            data: { companyId: companyId, name: name, code: code },
            success: function (result) {
                debugger;
                console.log(result.status);
                console.log(result.message);
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.MASTER, true);

            },
            error: function (e) {
                console.log(e);
                debugger;
            }
        });
    }
    else {
        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.MASTER, true);
    }
}



function CustomerDelete() {
    debugger;
    Focus8WAPI.getGlobalValue("CustomerCallbackDelete", '*', 1);
}

function CustomerCallbackDelete(response) {
    debugger;
    companyId = response.data.CompanyId;
    sessionId = response.data.SessionId;
    Focus8WAPI.getFieldValue("CustomerDataCallbackDelete", ["sName", "sCode","iAccountType"], 1, false, 111);
}

function CustomerDataCallbackDelete(response1) {
    debugger;
    name = response1.data[0].FieldValue;
    code = response1.data[1].FieldValue;
    Accounttype = response1.data[2].FieldValue;
    if (Accounttype == 5) {


        $.ajax({
            url: baseUrl + "/Master/CustomerDelete",
            type: "POST",
            data: { companyId: companyId, name: name, code: code },
            success: function (result) {
                debugger;
                alert(result.message)
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.MASTER, true);

            },
            error: function (e) {
                console.log(e);
                debugger;
            }
        });
    }
    else
    {
        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.MASTER, true);
    }
}





function AddressCreate() {
    debugger;
    Focus8WAPI.getGlobalValue("AddressCallback", '*', 1);
}

function AddressCallback(response) {
    debugger;
    companyId = response.data.CompanyId;
    sessionId = response.data.SessionId;
    Focus8WAPI.getFieldValue("AddressDataCallback", ["sName", "sCode","ID"], 1, false, 111);
}

function AddressDataCallback(response1) {
    debugger;
    name = response1.data[0].FieldValue;
    code = response1.data[1].FieldValue;  
    Id = response1.data[2].FieldValue;

        $.ajax({
            url: baseUrl + "/Master/AddressData",
            type: "POST",
            data: { companyId: companyId, name: name, code: code,Id:Id },
            success: function (result) {
                debugger;
                //alert(result.message)
                console.log(result.status);
                console.log(result.message);
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.MASTER, true);

            },
            error: function (e) {
                console.log(e);
                debugger;
            }
        });
   
}



function AddressDelete() {
    debugger;
    Focus8WAPI.getGlobalValue("AddressCallbackDelete", '*', 1);
}

function AddressCallbackDelete(response) {
    debugger;
    companyId = response.data.CompanyId;
    sessionId = response.data.SessionId;
    Focus8WAPI.getFieldValue("AddressDataCallbackDelete", ["sName", "sCode"], 1, false, 111);
}

function AddressDataCallbackDelete(response1) {
    debugger;
    name = response1.data[0].FieldValue;
    code = response1.data[1].FieldValue;
        $.ajax({
            url: baseUrl + "/Master/AddressDelete",
            type: "POST",
            data: { companyId: companyId, name: name, code: code },
            success: function (result) {
                debugger;
                alert(result.message)
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.MASTER, true);

            },
            error: function (e) {
                console.log(e);
                debugger;
            }
        });
    
}
