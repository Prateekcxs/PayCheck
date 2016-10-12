$(document).ready(function () {
    $("#SalaryYear").change(function () {
        $("#SelectedYear").val($(this).val());
        
       // alert($("#SelectedYear").val());
        $("#GeneratePaySlip").submit();
    });
});