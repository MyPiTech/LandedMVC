$(function () {
    //table
    DT_ACTION_COLUMN.mRender = actions;
    $.fn.dataTable.ext.errMode = dataTableError;

    table = $('#dataTable').DataTable({
        order: DT_ORDER,
        pagingType: DT_PAGING_TYPE,
        pageLength: DT_PAGE_LENGTH,
        lengthMenu: DT_LENGTH_MENU,
        columns: DT_COLUMNS,
        ajax: DT_AJAX,
        responsive: true,
        stateSave: true,
        createdRow: addRowId,
        fnDrawCallback: hideTablePagination
    });

    //form
    fetchUsersAsync();

    validator = $('#dataForm').validate({
        rules: V_RULES,
        messages: V_MESSAGES,
        submitHandler: formHandler
    });

    $.validator.addMethod('inUserArray', function (value, element) {
        const valid = users.some(u => u.name === value);
        return this.optional(element) || valid;
    }, 'You must enter/select the name of an exisitng user.');

});