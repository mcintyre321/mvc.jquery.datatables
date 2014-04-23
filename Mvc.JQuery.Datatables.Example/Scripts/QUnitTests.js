; (function ($) {
    var debug = true,
        queryStringToObject = function (queryString) {
            var vars = queryString.split('&'),
                returnVar = {};
            for (var i = 0; i < vars.length; i++) {
                var pair = vars[i].split('='),
                    propName = decodeURIComponent(pair[0]),
                    val = decodeURIComponent(pair[1]),
                    match = /^(\w+)_(\d+)$/.exec(propName);
                if (match) {
                    propName = match[1];
                    if (!$.isArray(returnVar[propName])) {
                        returnVar[propName] = [];
                    }
                    returnVar[propName][parseInt(match[2])] = val;
                } else {
                    returnVar[propName] = val;
                }
            }
            return returnVar;
        },
        // could also try http://stackoverflow.com/questions/12003978/how-to-overwrite-the-success-function-via-jquery-ajaxsend-event#12010724
        dTAjaxController = (function () {
            var pushStack = [], mostRecentArgs, returnVar = {},
                fnSend = function (event, jqXHR, ajaxOptions) {
                    if (!mostRecentArgs) { return; }
                    var match = /\bsEcho=(\d+)/.exec(ajaxOptions.data), sEcho;
                    if (mostRecentArgs.onSend && (match || mostRecentArgs.runSendIfNoEcho)) {
                        mostRecentArgs.onSend.apply(this, arguments);
                    }
                    if (!match) { return; }
                    sEcho = parseInt(match[1]);
                    pushStack[sEcho] = {
                        onSuccess:mostRecentArgs.onSuccess, 
                        onComplete: mostRecentArgs.onComplete
                    }
                    mostRecentArgs = null;
                },
                fnSuccess = function (event, XMLHttpRequest, ajaxOptions){
                    var sEcho = parseInt(XMLHttpRequest.responseJSON.sEcho);
                    if (!isNaN(sEcho) && pushStack[sEcho] && pushStack[sEcho].onSuccess) {
                        pushStack[sEcho].onSuccess.apply(this, arguments);
                    }
                },
                fnComplete = function (event, XMLHttpRequest, ajaxOptions) {
                    var sEcho = parseInt(XMLHttpRequest.responseJSON.sEcho);
                    if (!isNaN(sEcho) && pushStack[sEcho]) {
                        if (pushStack[sEcho].onComplete) {
                            pushStack[sEcho].onComplete.apply(this, arguments);
                        }
                        delete pushStack[sEcho];
                    }
                };

            $(document).ajaxSend(fnSend)
                .ajaxSuccess(fnSuccess)
                .ajaxComplete(fnComplete);

            returnVar.add = function (argObj) {
                var acceptableTypes = ["undefined", "function"];
                if (debug) {
                    var functionArgs = ["trigger", "onSend", "onSuccess", "onComplete"],
                        allArgs = functionArgs.concat(["runSendIfNoEcho"]), p;
                    if (!argObj) {
                        throw new Error("at least 1 argument must be provided");
                    }
                    if (!argObj.trigger) {
                        throw new Error("a trigger function must be provided");
                    }
                    $.each(functionArgs, function (indx,fn) {
                        if (acceptableTypes.indexOf(typeof (argObj[fn])) == -1) {
                            throw new TypeError(fn + " must be a function");
                        }
                    });
                    for (p in argObj) {
                        if (argObj.hasOwnProperty(p) && $.inArray(p, allArgs) == -1) {
                            throw new Error("undefined argument '" + p + "' passed to ajaxController.add method");
                        }
                    }
                }
                //do not trigger until last mostRecentArgs consumed
                (function waitUnitArgsConsumed() {
                    if (mostRecentArgs) {
                        setTimeout(waitUnitArgsConsumed,20);
                        return;
                    }
                    mostRecentArgs = argObj;
                    argObj.trigger() ;
                })();
            }

            returnVar.cleanup = function () {
                pushStack = null;
                $(document).off("ajaxSend", fnSend)
                    .off("ajaxSuccess", fnSuccess)
                    .off("ajaxComplete", fnComplete);
            }

            return returnVar;
        })();

    QUnit.done(dTAjaxController.cleanup);
    QUnit.config.testTimeout = 30000;
    QUnit.testStart(function resetElements() {
        var context = document.getElementById("testTableHolder");
        $("input[type='text']:enabled", context)
            .each(function () {
                if (this.value !== "") {
                    this.value = "";
                    $(this).triggerHandler("keyup");
                }
            });
        $("input[type='checkbox']:enabled", context)
            .each(function () {
                if (this.checked) {
                    this.checked = false;
                    $(this).triggerHandler("click");
                }
        });
        $("select:enabled", context)
            .each(function () {
                $(this).children().filter(function (indx) {
                    return (indx == 0 || this.defaultSelected)
                }).last()
                    .each(function () {
                        if (!this.selected) {
                            this.selected = true;
                            this.triggerHandler("change");
                        }
                    });
        });
    });

    $(document).ajaxError(function(event, jqXHR, ajaxSettings, thrownError)
    {
        test("ajax has returned with an error",function(){
            ok(false,"error returned: " + thrownError);
        });
    });//not cleaned up at present

    QUnit.assert.isNumeric = function( value, message ) {
        var number, isNumeric;
        if (isFinite(value.length) && typeof (value) != "string") {
            var i = 0;
            isNumeric = true;
            for(;i<value.length; i++){
                number = parseFloat(value[i]);
                if (isNaN(number)) {
                    isNumeric = false;
                    break;
                }
            }
        } else {
            number = parseFloat(value);
            isNumeric = !isNaN(number)
        }
        QUnit.push(isNumeric, value, "numbers only", message); /*result, actual, expected, message*/
    };

    module("initialise and populate dataTable");

    test("datatable applied & thead input elements placed (columnFilter)", function () {
        var $testTbl = $($.fn.dataTable.fnTables()[0]),
            $dtHead = $testTbl.children('thead');
        //test datatable has been applied
        expect(4);
        ok($testTbl.length && $testTbl[0].tagName.toLowerCase() == 'table', "an instantiated datatable is expected to be found before window.onLoad");
        //test appropriate input elements are placed
        equal($dtHead.find('input').filter(".hasDatepicker").length, 2, "2 datepicker elements are expected");
        equal($dtHead.find('select').length, 2, "2 select elements are expected");
        ok($dtHead.find('button').length, "a button element is expected");
    });

    asyncTest("data sent to and returned from server is OK", function (assert) {
        expect(12);
        stop();
        dTAjaxController.add({
            runSendIfNoEcho: true,
            trigger: function () {
                $($.fn.dataTable.fnTables()[0]).dataTable().fnDraw();
            },
            onSend: function (event, jqXHR, ajaxOptions) {
                var queryObj;
                ok(ajaxOptions.data && ajaxOptions.data.length, "ajax request contains data");
                queryObj = queryStringToObject(ajaxOptions.data);
                notEqual(queryObj.sEcho, null, "sEcho must have a value");
                assert.isNumeric(queryObj.iColumns, "iColumns must be a number");
                ["bRegex", "bSearchable", "bSortable", "mDataProp", "sSearch"].forEach(function (prop) {
                    equal(queryObj[prop].length, queryObj.iColumns, prop + ".length == iColumns");
                });
                start();
            },
            onComplete: function (event, XMLHttpRequest, ajaxOptions) {
                var listIds,
                    tableIds,
                    listData = XMLHttpRequest.responseJSON.aaData,
                    $testTbl = $($.fn.dataTable.fnTables()[0]);
                ok($.isArray(listData) && listData.length, "an array returned from server contains elements");
                listIds = $.map(listData, function (val) {
                    return val[0];
                });
                assert.isNumeric(listIds, "elements[0..N][0] of returned array are numeric");
                tableIds = $.map($testTbl.children("tbody").children(), function (tr) {
                    return $(tr).children().first().text();
                });
                assert.isNumeric(tableIds, "first column of table contains only numeric text");
                deepEqual(tableIds, listIds, "JSON translated to table");
                start();
            }
        });
    });

    module("emmulate user input");

    asyncTest("sort on click", function () {
        var $testTbl = $($.fn.dataTable.fnTables()[0]),
            $id = $testTbl.children('thead').children().children(":contains('Id')"),
            triggerClick = function () {
                $id.trigger("click");
            },
            ordered = $.map($(Array(10)), function (val, i) { return i+1+""; }),
            reverseOrdered = $.map($(Array(10)), function (val, i) { return 100-i+""; }),
            secondClickExpect;
        expect(2);
        stop();
        dTAjaxController.add({
            trigger:triggerClick,
            onSuccess:function(event, XMLHttpRequest, ajaxOptions) {
                var returned = $.map(XMLHttpRequest.responseJSON.aaData, function (val) {
                        return val[0];
                }),
                    expected;
                if (returned[0]==100){
                    expected = reverseOrdered;
                    secondClickExpect = ordered;
                } else {
                    expected = ordered;
                    secondClickExpect = reverseOrdered;
                }
                deepEqual(returned,expected,"clicking on Id sorts table");
                start();
            }});
        dTAjaxController.add({
            trigger:triggerClick,
            onSuccess: function (event, XMLHttpRequest, ajaxOptions) {
                (function secondClick() {
                    if (!secondClickExpect) {
                        setTimeout(secondClick, 20);
                        return;
                    }
                    var returned = $.map(XMLHttpRequest.responseJSON.aaData, function (val) {
                        return val[0];
                    });
                    deepEqual(returned, secondClickExpect, "clicking again on Id again alternates table sort");
                    start();
                })();
            }});
    });

    asyncTest("search", function () {
        var $search = $("label:contains('Search'):first").children('input');
        expect(2);
        stop();
        dTAjaxController.add({
            trigger: function () {
                $search.val("99")
                    .trigger("keyup");
            },
            onSuccess: function (event, XMLHttpRequest, ajaxOptions) {
                var returned = $.map(XMLHttpRequest.responseJSON.aaData, function (val) {
                    return val[0];
                });
                deepEqual(returned, ["99"], "searching for 99 returns 1 record");
                start();
            }
        });
        dTAjaxController.add({
            trigger: function () {
                $search.val("user11@hotmail.com")
                    .trigger("keyup");
            },
            onSuccess: function (event, XMLHttpRequest, ajaxOptions) {
                var returned = $.map(XMLHttpRequest.responseJSON.aaData, function (val) {
                    return val[0];
                });
                deepEqual(returned, [], "searching for an email does not return a record");
                start();
            }
        });
    });
})(jQuery);