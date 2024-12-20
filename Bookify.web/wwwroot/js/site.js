var table;
var datatable;
var updatedRow;
var exportedCols = []

function showSuccessMessage(message = "Saved Successfully") {
    Swal.fire({
        text: message,
        icon: "success",
        buttonsStyling: false,
        confirmButtonText: "Ok",
        customClass: {
            confirmButton: "btn btn-primary"
        }
    });
}

function showErrorMessage(message = "Somthing Went Wrong!") {
    Swal.fire({
        text: message,
        icon: "error",
        buttonsStyling: false,
        confirmButtonText: "Ok",
        customClass: {
            confirmButton: "btn btn-danger"
        }
    });
}

function onModalBegin() {
    $('body :submit').attr('disabled', 'disabled').attr('data-kt-indicator', 'on');
}

function onModalSuccess(row) {
    showSuccessMessage();
    $('#modal').modal('hide');

    if (updatedRow !== undefined) {
        datatable.row(updatedRow).remove().draw();
        updatedRow = undefined;
    } 

    var newRow = $(row)
    datatable.row.add(newRow).draw();

    KTMenu.init()
    KTMenu.initHandlers()
}
function onModalComplete() {
    $('body :submit').removeAttr('disabled').removeAttr('data-kt-indicator');
}


// DataTables
var headers = $('th');
$.each(headers, function(i) {
    var col = $(this);
    if (!col.hasClass('js-no-export'))
        exportedCols.push(i);
})

var KTDatatables = function () {
    var initDatatable = function () {
        datatable = $(table).DataTable({
            "info": false,
            'pageLength': 10,
        });
    }

    var exportButtons = () => {
        const documentTitle = $('.js-DataTables').data('document-title');
        var buttons = new $.fn.dataTable.Buttons(table, {
            buttons: [
                {
                    extend: 'copyHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                },
                {
                    extend: 'excelHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                },
                {
                    extend: 'csvHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                },
                {
                    extend: 'pdfHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                }
            ]
        }).container().appendTo($('#kt_datatable_example_buttons'));

        const exportButtons = document.querySelectorAll('#kt_datatable_example_export_menu [data-kt-export]');
        exportButtons.forEach(exportButton => {
            exportButton.addEventListener('click', e => {
                e.preventDefault();

                const exportValue = e.target.getAttribute('data-kt-export');
                const target = document.querySelector('.dt-buttons .buttons-' + exportValue);

                target.click();
            });
        });
    }

    // Search Datatable --- official docs reference: https://datatables.net/reference/api/search()
    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[data-kt-filter="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    }

    // Public methods
    return {
        init: function () {
            table = document.querySelector('.js-DataTables');

            if (!table) {
                return;
            }

            initDatatable();
            exportButtons();
            handleSearchDatatable();
        }
    };
}();
$(document).ready(function () {
    //sweet Alerts
    var message = $('#message').text();
    if (message != '') {
        showSuccessMessage(message);
    }

    //DataTables
    KTUtil.onDOMContentLoaded(function () {
        KTDatatables.init();
    });

    // Handel Modal
    $('body').delegate('.js-render-modal', 'click', function () {
        var btn = $(this);
        var modal = $('#modal');

        modal.find('#modalTitle').text(btn.data('title'));

        if (btn.data('update') !== undefined) {
            updatedRow = btn.parents('tr');
        }

        $.ajax({
            type: 'get',
            url: btn.data('url'),
            success: function (form) {
                modal.find('#modalBody').html(form);
                $.validator.unobtrusive.parse(modal)
            },
            error: function () {
                showErrorMessage();
            }
        })
        modal.modal('show');
    })

    // Toddle Status
    $('body').delegate('.js-toggle-status', 'click', function () {
        var btn = $(this)
        bootbox.confirm({
            message: "Are you sure you want to toggle that status?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-danger'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-secondary'
                }
            },
            callback: function (result) {
                if (result) {
                    $.ajax({
                        type: "POST",
                        url: btn.data('url'),
                        data: {
                            '__RequestVerificationToken': $('input[name=__RequestVerificationToken]').val()
                        },
                        success: function (LastUpdatedOn) {
                            var row = btn.parents('tr');
                            var status = row.find('.js-status');
                            var newText = status.text().trim() === 'Deleted' ? 'Available' : 'Deleted';
                            status.text(newText).toggleClass('badge-light-danger badge-light-success');
                            row.find('.js-LastUpdatedOn').html(LastUpdatedOn);
                            row.addClass("animate__animated animate__fadeIn");
                            showSuccessMessage();

                        },
                        error: function () {
                            ShowErrorMessage();
                        }
                    });
                }
            }
        });


    });
});
