// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $("#Products").on("change", function () {
        $list = $("#PermissionProfilesFiltered");
        $.ajax({
            url: "/getPermissionProfiles",
            type: "GET",
            data: { productId: $("#Products").val() },
            traditional: true,
            success: function (result) {
                $list.empty();
                $.each(result, function (i, item) {
                    $list.append('<option value="' + item["permissionProfileId"] + '"> ' + item["permissionProfileName"] + ' </option>');
                });
            },
            error: function () {
                console.warn("Issues with calling the getPermissionProfiles endpoint. Please, check the solution.");
            }
        });
    });
});
