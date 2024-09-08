export default function FormValidator(valState, setValState) {

    const schema = {};

    return {
        onField: function (fieldName, value) {
            schema[fieldName] = [];

            return {
                required: function (message) {
                    schema[fieldName].push(() => { 
                        const result = value != null;
                        setValState({...valState, [fieldName]: !result ? message : null});
                    });

                    return this;
                },

                matches: function (regex, message) {
                    schema[fieldName].push(() => {
                        const result = new RegExp(regex).test(value || '');
                        setValState({...valState, [fieldName]: !result ? message : null});
                    })
                },

                equal: function (v, message) {
                    schema[fieldName].push(() => {
                        const result = v == value;
                        setValState({...valState, [fieldName]: !result ? message : null});
                    })
                },

                notEqual: function (v, message) {
                    schema[fieldName].push(() => {
                        const result = v != value;
                        setValState({...valState, [fieldName]: !result ? message : null});
                    })
                },

                lessThan: function (v, message) {
                    schema[fieldName].push(() => {
                        const result = value < v;
                        setValState({...valState, [fieldName]: !result ? message : null});
                    })
                },

                lessThanOrEqual: function (v, message) {
                    schema[fieldName].push(() => {
                        const result = value <= v;
                        setValState({...valState, [fieldName]: !result ? message : null});
                    })
                },

                greaterThan: function (v, message) {
                    schema[fieldName].push(() => {
                        const result = value > v;
                        setValState({...valState, [fieldName]: !result ? message : null});
                    })
                },

                greaterThanOrEqual: function (v, message) {
                    schema[fieldName].push(() => {
                        const result = value >= v;
                        setValState({...valState, [fieldName]: !result ? message : null});
                    })
                },

                must: function (callback, message) {
                    schema[fieldName].push(() => {
                        const result = callback();
                        setValState({...valState, [fieldName]: !result ? message : null});
                    })
                },

                mustAsync: function (callback, message) {
                    schema[fieldName].push(async () => {
                        const result = await callback();
                        setValState({...valState, [fieldName]: !result ? message : null});
                    })
                }
            }
        },

        validate: function () {
            const fieldNames = Object.keys(schema);
            for (let fieldName of fieldNames) {
                for (let func of schema[fieldName]) {
                    func();
                    if (valState[fieldName] != null)
                        break;
                }
            }
        },

        isFieldValid: function (name) {
            return valState[name] == null;
        },

        isFieldError: function (name) {
            return valState[name] == null;
        },

        validateField: function (name) {
            for (let func of schema[name]) {
                func();
                if (valState[name] != null)
                    break;
            }
        }
    };
}

