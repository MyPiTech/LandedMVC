//table
let table = null;

const DT_COLUMNS = [
    DT_CONTROL_COLUMN,
    DT_ID_COLUMN,
    {
        data: 'title',
        render: DataTable.render.ellipsis(10)
    },
    {
        data: 'location',
        render: DataTable.render.ellipsis(10)
    },
    {
        data: 'start',
        render: DataTable.render.datetime()
    },
    { data: 'duration' },
    DT_ACTION_COLUMN
];
const DT_AJAX = { url: `/users/getAllEvents?uId=${uId}`, dataSrc: '' };

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
