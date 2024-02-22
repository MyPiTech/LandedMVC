

$(function () {
    $('#usersTbl').DataTable({
        pagingType: 'first_last_numbers',
        pageLength: 5,
        lengthMenu: [5, 10, 25, 50, 75, 100],
        ajax: { url: '/users/users', dataSrc: '' },
        columns: [
            { data: 'id' },
            { data: 'lastName' },
            { data: 'firstName' },
            {
                data: null,
                orderable: false,
                mRender: function (data, type, row) {
                    return '<a href="/events?userId=' + data.id + '" class="icon solid alt fa-calendar" title="Events"></a>&nbsp;&nbsp;&nbsp;' +
                        '<a href="/events?userId=' + data.id + '" class="icon solid alt fa-list-alt" title="Edit User"></a>&nbsp;&nbsp;&nbsp;' +
                        '<a href="/events?userId=' + data.id + '" class="icon solid alt fa-trash" title="Delete User"></a>';
                }
            }
        ],
        fnDrawCallback: function (oSettings) {
            if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                $(oSettings.nTableWrapper).find('.dataTables_paginate').hide();
            } else {
                $(oSettings.nTableWrapper).find('.dataTables_paginate').show();
            }
        }
    });
});