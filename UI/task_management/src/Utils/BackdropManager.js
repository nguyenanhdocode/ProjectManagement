export default function BackdropManager(dispatch) {
    return {
        show: function () {
            dispatch({type: 'show'});
        },

        hide: function () {
            dispatch({type: 'hide'});
        }
    };
}