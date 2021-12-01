/* eslint-disable no-useless-escape */
import 'date-fns';

import {
  KeyboardDateTimePicker,
  MuiPickersUtilsProvider,
} from '@material-ui/pickers';
import React, { useEffect, useState } from 'react';

import DateFnsUtils from '@date-io/date-fns';
import Grid from '@material-ui/core/Grid';
import deLocale from 'date-fns/locale/pt-BR';
import dayjs from 'dayjs';
import utc from 'dayjs/plugin/utc';
import timezone from 'dayjs/plugin/timezone';
import Icon from '@mdi/react';
import { mdiCalendarClock } from '@mdi/js';

import './timepicker.css';

const MaterialUIPickers = (props) => {
  dayjs.extend(utc);
  dayjs.extend(timezone);
  const state = props;
  const [selectedDate, setSelectedDate] = useState(null);
  const dados = state;

  const formatarData = (data) => {
    const [dia, mes, ano, hora] = data.split(/[\/\s]/);
    return `${ano}-${`0${mes}`.slice(-2)}-${`0${dia}`.slice(-2)}T${hora}-03:00`;
  };

  useEffect(() => {
    setSelectedDate(state.data.value);
  }, [state]);

  useEffect(() => {
    const formato = 'YYYY-MM-DDTHH:mm-03:00';
    // const formato = '[YYYYescape] YYYY-MM-DDTHH:mm:ssZ[Z]';
    if (state.data.value) {
      const invalid =
        state.data.value !== 'Invalid Date' &&
        state.data.value !== 'Invalid date';
      if (state.data.value.length <= 16 && invalid) {
        setSelectedDate(formatarData(state.data.value));
        return;
      }
      if (!state.data.value.length && invalid) {
        setSelectedDate(
          dayjs(state.data.value).tz('America/Sao_Paulo').format(formato),
        );
      }
      return;
    }

    if (!(typeof state.data.edicao === 'undefined')) {
      setSelectedDate(
        !state.data.edicao
          ? dayjs().tz('America/Sao_Paulo').format(formato)
          : state.data.value,
      );
      return;
    }

    setSelectedDate(state.data.value);
  });

  const handleDateChange = (date) => {
    state.onChange(
      state.data.name,
      dayjs(date).tz('America/Sao_Paulo').format('YYYY-MM-DDTHH:mm-03:00'),
    );
    setSelectedDate(date);
  };

  //   const handleError = (error, value) => {
  //     console.log(error + value);
  //   };

  return (
    <MuiPickersUtilsProvider utils={DateFnsUtils} locale={deLocale}>
      <Grid container justify="space-around">
        <KeyboardDateTimePicker
          ampm={false}
          variant="dialog"
          format="dd/MM/yyyy"
          margin="normal"
          id="date-picker-dialog"
          label={dados.data.label}
          value={selectedDate}
          onChange={handleDateChange}
          keyboardIcon={
            <Icon
              path={mdiCalendarClock}
              title="Pesquisar"
              size={1}
              color="black"
            />
          }
          showTodayButton
          todayLabel="Hoje"
          cancelLabel="Cancelar"
          style={{ marginTop: 0, color: 'black' }}
          fullWidth
          invalidDateMessage="Data/Hora inválida!"
        />
      </Grid>
    </MuiPickersUtilsProvider>
  );
};

export default MaterialUIPickers;
