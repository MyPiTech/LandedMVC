//table
let table = null;

const DT_COLUMNS = [
    DT_CONTROL_COLUMN,
    DT_ID_COLUMN,
    { data: 'userId' },
    {
        data: 'title',
        render: DataTable.render.ellipsis(10),
        className: 'tdNoWrap'
    },
    {
        data: 'location',
        render: DataTable.render.ellipsis(10),
        className: 'tdNoWrap'
    },
    {
        data: 'start',
        render: DataTable.render.datetime(),
        className: 'tdNoWrap'
    },
    { data: 'duration' },
    DT_ACTION_COLUMN
];
const DT_AJAX = { url: '/events/getAll', dataSrc: '' };

//form
let validator = null;
let formState = null;

const V_RULES = {
    userInput: {
        required: true,
        inUserArray: true
    },
    title: {
        required: true
    },
    start: {
        required: true
    },
    duration: {
        required: true
    }
};
const V_MESSAGES = {
    userInput: {
        required: 'You must enter/select the name of an exisitng user.'
    },
    title: {
        required: 'Please enter an event title.'
    },
    start: {
        required: 'Please enter a valid start date/time.'
    },
    duration: {
        required: 'Please enter a duration.'
    }
};
