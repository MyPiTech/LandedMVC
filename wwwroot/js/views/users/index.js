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
    validator = $('#dataForm').validate({
        rules: V_RULES,
        messages: V_MESSAGES,
        submitHandler: formHandler
    });

    //event handlers
    $('#dataTable tbody').on('click', 'span', actionsHandler);

    $('#createBtn').on('click', createHandler);

    $('#resetBtn').on('click', resetHandler);

    $('#cancelBtn').on('click', cancelHandler);

    $('#labelsToggle').on('click', lablesHandler);

});

