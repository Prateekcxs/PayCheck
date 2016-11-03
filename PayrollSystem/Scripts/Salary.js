$(document).ready(function () {
    $("#SalaryYear").change(function () {
        $("#SelectedYear").val($(this).val());


        $.get('/Salary/MonthList/?year=' + $("#SelectedYear").val(), function (data) {

            /* data is the pure html returned from action method, load it to your page */
            $('#MonthList').html(data);

            //console.log(data);
        });

        if ($(this).val() == '') {
            $("#BtnGeneratePaySlip").hide();
        }
    });
});

window.onbeforeunload = function ()
{
    $('#loader').show();
    $("#gtco-header :input").attr("disabled", true);
}

    function ToggleGenerateBtn() {

        var atLeastOneIsChecked = $('input[name="chkMonthList"]:checked').length > 0;

       // console.log(atLeastOneIsChecked);
        if(atLeastOneIsChecked)
        {
            $("#BtnGeneratePaySlip").show();
        }
        else
        {
            $("#BtnGeneratePaySlip").hide();
        }
    }