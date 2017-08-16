$(document).ready(function () {
    var dataTable, dialog, form;

    dataTable = $('#myTable').DataTable({
        processing: true, // for show progress bar
        serverSide: true, // for process server side
        filter: false, // this is for disable filter (search box)
        orderMulti: false, // for disable multiple column at once
        ajax: {
            url: "/Lists/LoadDataPaged",
            type: "POST",
            datatype: "json"
        },
        cache: false,
        columns: [
            { "data": "listTitle", "name": "listTitle", "title": "List Title", "autoWidth": true }
            , {
                "data": "Test",
                "name": "Test",
                "visible": true,
                "title": "Edit",
                "searchable": false,
                "orderable": false,
                "render": function (data, type, full, meta) {
                    return "<input type='button' value='Edit " + full.listTitle + "' data-list-id='" + full.listId + "' data-list-title='" + full.listTitle + "''></input>";
                }
            }
            //{ "data": "CompanyName", "autoWidth": true },
            //{ "data": "Phone", "autoWidth": true },
            //{ "data": "Country", "autoWidth": true },
            //{ "data": "City", "autoWidth": true },
            //{ "data": "PostalCode", "autoWidth": true }
        ],
        initComplete: function (settings, json) {

        }
    });

    $('#myTable').on('draw.dt', function () {
        $('input').on('click', function () {
            dialog.data('ListTile', $(this).attr('data-list-title'));
            dialog.data('ListId', $(this).attr('data-list-id'));

            dialog.dialog("open");
        })
    });

    function updateListTitle() {
        var valid = true;

        var values = {};
        $.each($('#dialog-form input').serializeArray(), function (i, field) {
            values[field.name] = field.value;
        });
        //alert(JSON.stringify(values));

        $.ajax({
            method: "POST",
            url: "/ListsAPI/UpdateListTitle",
            data: values
        })
            .done(function (msg) {
                //alert("Data Saved: " + JSON.stringify(msg));
                dataTable.ajax.reload(null, false);
            });
        dialog.dialog("close");
        return valid;
    }

    dialog = $("#dialog-form").dialog({
        autoOpen: false,
        height: 400,
        width: 350,
        modal: true,
        open: function (event, ui) {
            $('#listTitle').val(dialog.data('ListTile'));
            $('#listID').val(dialog.data('ListId'));
        },
        buttons: {
            "Update": updateListTitle,
            Cancel: function () {
                dialog.dialog("close");
            }
        },
        close: function () {
            form[0].reset();
            //allFields.removeClass("ui-state-error");
            //$('#myTable').columns.adjust().draw();


        }
    });

    form = dialog.find("form").on("submit", function (event) {
        event.preventDefault();
        //addUser();
        var values = {};
        $.each($('#dialog-form input').serializeArray(), function (i, field) {
            values[field.name] = field.value;
        });
        alert(JSON.stringify(values));
    });

});








