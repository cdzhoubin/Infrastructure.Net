var Qhjsw = {};
Qhjsw.IDChecks = function (Code) {
    if (typeof Code != "string") {
        throw new Error("身份证编号必须是字符串！");
    }
    var city = { 11: "北京", 12: "天津", 13: "河北", 14: "山西", 15: "内蒙古", 21: "辽宁", 22: "吉林", 23: "黑龙江 ", 31: "上海", 32: "江苏", 33: "浙江", 34: "安徽", 35: "福建", 36: "江西", 37: "山东", 41: "河南", 42: "湖北 ", 43: "湖南", 44: "广东", 45: "广西", 46: "海南", 50: "重庆", 51: "四川", 52: "贵州", 53: "云南", 54: "西藏 ", 61: "陕西", 62: "甘肃", 63: "青海", 64: "宁夏", 65: "新疆", 71: "台湾", 81: "香港", 82: "澳门", 91: "国外 " };
    var tip = {};   //创建一个空对象，此对象用来返回验证结果
    var IDCode = Code;
    var code = [];  //创建一个空数组，用来保存被分割的身份证
    var sextable = ["女", "男"];   //创建数组，保存性别。表驱动
    var Ck = function (IDCode) {
        if (!IDCode || !/(^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$)|(^\d{6}(18|19|20){1}\d{2}(0[1-9]|1[012])(0[1-9]|[12]\d|3[01])\d{3}(\d|X|x)$)/i.test(IDCode)) {
            tip.msg = "身份证号格式错误";
            tip.pass = false;
        }
        else if (!city[IDCode.substr(0, 2)]) {
            tip.msg = "地址编码错误";
            tip.pass = false;
        }
        else if (!_CheckBirthday(IDCode)) {
            tip.msg = "身份证中出生日期不正确";
            tip.pass = false;
        }
        else {

            if (18 == IDCode.length) {
                code = IDCode.split('');
                var factor = [7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2];
                var parity = [1, 0, 'X', 9, 8, 7, 6, 5, 4, 3, 2];
                var sum = 0;
                var ai = 0;
                var wi = 0;
                if (typeof code[17] == "string") {
                    code[17] = code[17].toUpperCase();
                }
                for (var i = 0; i < 17; i++) {
                    ai = code[i];
                    wi = factor[i];
                    sum += ai * wi;
                }
                var last = parity[sum % 11];
                if (parity[sum % 11] != code[17]) {
                    tip.msg = "校验位错误";
                    tip.pass = false;
                }
                else {
                    tip.msg = "该身份证验证正确";
                    tip.pass = true;
                    tip.idcode = IDCode;
                    tip.address = city[IDCode.substr(0, 2)];
                    tip.sextag = _GetSex(IDCode);
                    tip.sex = sextable[tip.sextag];
                    tip.birthday = _GetBirthday(IDCode);

                }
            }
            else {
                tip.msg = "该身份证验证正确";
                tip.pass = true;
                tip.idcode = IDCode;
                tip.address = city[IDCode.substr(0, 2)];
                tip.sextag = _GetSex(IDCode);
                tip.sex = sextable[tip.sextag];
                tip.birthday = _GetBirthday(IDCode);
            }
        }
    };
    this.IDCk = function () {
        Ck(IDCode);
        return tip;
    };
    function _CheckBirthday(IDCodes) {
        if (IDCodes.length == 18) {
            var year = IDCodes.substring(6, 10);
            var month = IDCodes.substring(10, 12);
            var day = IDCodes.substring(12, 14);
            var temp_date = new Date(year, parseFloat(month) - 1, parseFloat(day));
            // 这里用getFullYear()获取年份，避免千年虫问题   
            if (temp_date.getFullYear() != parseFloat(year) || temp_date.getMonth() != parseFloat(month) - 1
                 || temp_date.getDate() != parseFloat(day)) {
                return false;
            }
            else {
                return true;
            }
        }
        else {
            var year = IDCodes.substring(6, 8);
            var month = IDCodes.substring(8, 10);
            var day = IDCodes.substring(10, 12);
            var temp_date = new Date(year, parseFloat(month) - 1, parseFloat(day));
            // 对于老身份证中的你年龄则不需考虑千年虫问题而使用getYear()方法   
            if (temp_date.getYear() != parseFloat(year) || temp_date.getMonth() != parseFloat(month) - 1
              || temp_date.getDate() != parseFloat(day)) {
                return false;
            }
            else {
                return true;
            }
        }
    }
    function _GetBirthday(IDCodes) {
        if (IDCodes.length == 18) {
            var year = IDCodes.substring(6, 10);
            var month = IDCodes.substring(10, 12);
            var day = IDCodes.substring(12, 14);
            return year + "-" + month + "-" + day;
        }
        else {
            var year = IDCodes.substring(6, 8);
            var month = IDCodes.substring(8, 10);
            var day = IDCodes.substring(10, 12);
            return year + "-" + month + "-" + day;

        }
    }
    function _GetSex(IDCodes) {
        if (IDCodes.length == 18) {
            if (IDCodes.substring(14, 17) % 2 == 0) {
                return 0;
            }
            else {
                return 1;
            }
        }
        else {
            if (IDCodes.substring(14, 15) % 2 == 0) {
                return 0;
            }
            else {
                return 1;
            }
        }
    }

};

function ValidateChinaIdCard(source, args) {
    var ck = new Qhjsw.IDChecks(args.Value);
    var tip = ck.IDCk();
    args.IsValid = tip.pass;
    if (!args.IsValid) {
        showErrorMessage(source, tip.msg);
    }
}

function ValidateOrganizationCode1997(source, args) {
    var tip = isValidEntpCode(args.Value);
    args.IsValid = tip.pass;
    if (!args.IsValid) {
        showErrorMessage(source, tip.msg);
    }
}

function showErrorMessage(source,msg) {
    if (source.innerText) {
        source.innerText = msg;
    } else {
        source.textContent = msg;
    }
}
function isValidEntpCode(code) {
    code = code.toUpperCase();
    var tip={};
    tip.pass = true;
    tip.msg = "";
    var ws = [3, 7, 9, 10, 5, 8, 4, 2];
    var str = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ';
    var reg = /^([0-9A-Z]){8}-[0-9|X]$/;
    if (!reg.test(code)) {
        tip.pass = false;
        tip.msg = "格式不正确，正确格式如：E0000000-X。";
        return tip;
    }
    var sum = 0;
    for (var i = 0; i < 8; i++) {
        sum += str.indexOf(code.charAt(i)) * ws[i];
    }
    var C9 = 11 - (sum % 11);
    if (C9 == 11) {
        C9 = '0';
    } else if (C9 == 10) {
        C9 = 'X';
    } 

    if (C9 != code.charAt(9)) {
        tip.pass = false;
        tip.msg = "校验位不正确。";
    }

    return tip;
}
