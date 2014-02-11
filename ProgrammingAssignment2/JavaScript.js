$(function () {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "WebService.asmx/AddWords",
        data: "{}",
        contenttype: "application/json",
        success: function () {
            $("#target").keyup(function () {
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "WebService.asmx/GetWords",
                    data: "{'word':'" + $("#target").val() + "'}",
                    contenttype: "application/json",
                    success: function (data) {
                        $('#contentDisplay').empty();
                        JSON.parse(data.d, function (key, value) {
                            $('#contentDisplay').append(value.replace(",","") + "<br>");
                        });
                    }
                });
            });
        }
    })

});

