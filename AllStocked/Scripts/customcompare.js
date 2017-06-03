
    jQuery.validator.addMethod("genericcompare", function (value, element, params) {
        // debugger;
        var propelename = params.split(",")[0];
        var operName = params.split(",")[1];
        if (params === undefined || params === null || params.length === 0 ||
            value === undefined || value === null || value.length === 0 ||
            propelename === undefined || propelename === null || propelename.length === 0 ||
            operName === undefined || operName === null || operName.length === 0) {
            //alert("test");
            return true;

        }
        var valueOther = $(propelename).val();
        var val1 = (isNaN(value) ? Date.parse(value) : eval(value));
        var val2 = (isNaN(valueOther) ? Date.parse(valueOther) : eval(valueOther));
        //alert("test2");

        if (operName === "GreaterThan") {
            //alert("Greaterthan3");
            return val1 > val2;
        }
        if (operName === "LessThan")
            return val1 < val2;
        if (operName === "GreaterThanOrEqual")
            return val1 >= val2;
        if (operName === "LessThanOrEqual")
            return val1 <= val2;
    })
    ; jQuery.validator.unobtrusive.adapters.add("genericcompare",
            ["comparetopropertyname", "operatorname"], function (options) {
                options.rules["genericcompare"] = "#" +
                    options.params.comparetopropertyname + "," + options.params.operatorname;
                options.messages["genericcompare"] = options.message;
            });