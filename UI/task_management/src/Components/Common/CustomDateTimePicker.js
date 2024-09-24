import * as React from "react";
import TextField from "@mui/material/TextField";
import Stack from "@mui/material/Stack";
import { Controller } from "react-hook-form";
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs'
import { DateTimePicker, MobileDatePicker, MobileDateTimePicker } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import { inputLabelClasses } from "@mui/material";

export default function CustomDateTimePicker({ 
    helperText, name, label, control, required, error, variant
    , inputRef, rules, defaultValue, labelShink = false, customOnChange,
    disabled, customOnClose
 }) {
    const [currentValue, setCurrentValue] = React.useState();

    return (
        <Stack sx={{ mt: 2 }}>
            <Controller
                name={name}
                control={control}
                defaultValue={defaultValue ? dayjs(defaultValue) : null}
                render={({ field: {ref, onChange, ...rest} }) => (
                    <MobileDateTimePicker slotProps={{textField: {
                        helperText: helperText,
                        error: error,
                        variant: variant,
                        inputRef: inputRef,
                        required: {required},
                        InputLabelProps: { shrink: labelShink },
                        ref: ref,
                        onChange: onChange,
                    }}}
                        
                        label={label}
                        value={currentValue}
                        onClose={() => {
                            if (typeof customOnClose === 'function')
                                customOnClose(currentValue);
                        }}
                        onChange={(newValue) => {
                            onChange(newValue);
                            setCurrentValue(newValue);
                            
                            if (typeof customOnChange === 'function')
                                customOnChange(new Date(newValue));
                        }}
                        disabled={disabled ? 'disabled' : ''}
                        {...rest}
                    />
                )}
            />
        </Stack>
    );
}