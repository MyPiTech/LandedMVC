//table
let table = null;

const DT_COLUMNS = [
    DT_CONTROL_COLUMN,
    DT_ID_COLUMN,
    {
        data: 'title',
        render: DataTable.render.ellipsis(10),
        className: 'td-nowrap'
    },
    {
        data: 'location',
        render: DataTable.render.ellipsis(10),
        className: 'td-nowrap'
    },
    {
        data: 'start',
        render: DataTable.render.datetime(),
        className: 'td-nowrap'
    },
    { data: 'duration' },
    DT_ACTION_COLUMN
];
const DT_AJAX = { url: `/users/${uId}/getAll`, dataSrc: '' };

//form
let validator = null;
let formState = null;
let users = null;

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
        required: true,
        step: 10
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
        required: 'Please enter a duration.',
        step: 'Must be a multiple of 10.'
    }
};
