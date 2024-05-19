//table
let table = null;

const DT_COLUMNS = [
    DT_CONTROL_COLUMN,
    DT_ID_COLUMN,
    {
        data: 'lastName',
        render: DataTable.render.ellipsis(10),
        className: 'td-nowrap'
    },
    {
        data: 'firstName',
        render: DataTable.render.ellipsis(10),
        className: 'td-nowrap'
    },
    {
        data: 'notes',
        orderable: false,
        render: DataTable.render.ellipsis(20),
        className: 'td-nowrap',
        type: 'string'
    },
    DT_ACTION_COLUMN
];
const DT_AJAX = { url: '/users/getAll', dataSrc: '' };

//form
let validator = null;
let formState = null;

const V_RULES = {
    firstName: {
        required: true
    },
    lastName: {
        required: true
    }
};
const V_MESSAGES = {
    firstName: {
        required: 'Please enter a first name.'
    },
    lastName: {
        required: 'Please enter a last name.'
    }
};
