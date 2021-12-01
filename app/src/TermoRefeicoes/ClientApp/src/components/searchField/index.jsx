/* eslint-disable no-unused-expressions */
import React, { useEffect, useState } from 'react';
import { makeStyles } from '@material-ui/core/styles';
import Grid from '@material-ui/core/Grid';
// import { AutoComplete } from 'material-ui';
import Autocomplete from '@material-ui/lab/Autocomplete';
import List from '@material-ui/core/List';
import Card from '@material-ui/core/Card';
import CardHeader from '@material-ui/core/CardHeader';
import ListItem from '@material-ui/core/ListItem';
import ListItemText from '@material-ui/core/ListItemText';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import Checkbox from '@material-ui/core/Checkbox';
import Divider from '@material-ui/core/Divider';
import TextField from '@material-ui/core/TextField';

const useStyles = makeStyles((theme) => ({
  root: {
    margin: 'auto',
  },
  cardHeader: {
    padding: theme.spacing(1, 2),
  },
  list: {
    width: 500,
    height: 400,
    backgroundColor: theme.palette.background.paper,
    overflow: 'auto',
  },
  button: {
    margin: theme.spacing(0.5, 0),
  },
}));

function not(a, b) {
  return a.filter((value) => b.indexOf(value) === -1);
}

function intersection(a, b) {
  return a.filter((value) => b.indexOf(value) !== -1);
}

function union(a, b) {
  return [...a, ...not(b, a)];
}

export default function SearchField(props) {
  // eslint-disable-next-line no-unused-vars
  const [state, setState] = useState(props);
  const classes = useStyles();
  const [checked, setChecked] = useState(state?.data.value);
  const { multiple } = state?.data;
  const left = state?.data.items;
  const [searchItem, setSearchItem] = useState([]);
  const [itemsArray, setItemsArray] = useState(state?.data.items);

  useEffect(() => {
    // setState(props);
    const { items } = state?.data;
    const filter = state?.data.value;
    const check = [];

    if (multiple) {
      filter &&
        filter.map((e, i) => {
          return (
            items &&
            items.filter(
              (s) => s.id.toString() === e.toString() && (check[i] = s),
            )
          );
        });
    } else {
      items &&
        items.filter(
          (s) => s.id.toString() === filter.toString() && (check[0] = s),
        );
    }
    setSearchItem(check);
    setChecked(check);
  }, []);

  const handleToggle = (value) => () => {
    const todos = [];
    let apenasUm;
    const currentIndex = checked.indexOf(value);
    const currentIndexItem = searchItem.indexOf(value);
    const newChecked = [...checked];
    const newSearchItem = [...searchItem];

    if (currentIndex === -1) {
      newChecked.push(value);
    } else {
      newChecked.splice(currentIndex, 1);
    }

    if (currentIndexItem === -1) {
      newSearchItem.push(value);
    } else {
      newSearchItem.splice(currentIndexItem, 1);
    }
    !multiple && newChecked.length > 1 && newChecked.shift();

    newChecked.map((e, i) => {
      if (e) {
        todos[i] = e.id;
        apenasUm = e.id.toString();
      }
      return apenasUm;
    });
    setSearchItem(newSearchItem);
    setChecked(newChecked);
    state?.onChange(state?.data.name, multiple ? todos : apenasUm);
  };

  function searchItems(descricao) {
    const { items } = state?.data;
    const search = [];

    items &&
      items.filter((s, i) => {
        const name =
          (s.name && s.name.toUpperCase()) ||
          (s.descricao && s.descricao.toUpperCase());
        return name.includes(descricao.toUpperCase()) && (search[i] = s);
      });

    // setSearchItem(descricao);
    setItemsArray(search);
  }

  const numberOfChecked = (items) =>
    multiple && intersection(checked, items).length;
  const handleToggleAll = (items) => () => {
    const todos = [];
    if (numberOfChecked(items) === items.length) {
      setChecked(not(checked, items));
      setSearchItem(not(checked, items));
      state?.onChange(state?.data.name, todos);
    } else {
      setChecked(union(checked, items));

      items.map((e, i) => (todos[i] = e.id));
      setSearchItem(union(checked, items));

      state?.onChange(state?.data.name, todos);
    }
  };

  const onChange = (value) => {
    //   const todos = [];
    //   const newChecked = [...checked];
    //   // const newSearchItem = [...searchItem];
    //   // const currentIndexItem = searchItem.indexOf(v);
    //   // newChecked.push(v);
    //   // if (currentIndexItem === -1) {
    //   //   newSearchItem.push(v);
    //   // } else {
    //   //   newSearchItem.splice(currentIndexItem, 1);
    //   // }
    //   handleToggle(v);
    //   newChecked.map((e, i) => {
    //     if (e) {
    //       // const dados = {};
    //       // dados.id = e.id;
    //       // dados.descricao = e.descricao;
    //       todos[i] = e.id;
    //       // todos[i].descricao = e.descricao;
    //     }
    //     return todos;
    //   });

    // setChecked(newChecked);
    // setSearchItem(newSearchItem);
    //   state?.onChange(state?.data.name, todos);

    const todos = [];
    let apenasUm;
    let currentIndex;
    if (value.length > 0) {
      value.forEach((v) => {
        currentIndex = searchItem.indexOf(v);

        // const currentIndexItem = searchItem.indexOf(value);
        // const newChecked = [...checked];
        const newSearchItem = [...searchItem];

        if (currentIndex === -1) {
          // newChecked.push(value);
          newSearchItem.push(v);
        } else {
          newSearchItem.splice(currentIndex, 1);
        }

        // if (currentIndexItem === -1) {
        // } else {
        //   newSearchItem.splice(currentIndexItem, 1);
        // }
        !multiple && newSearchItem.length > 1 && newSearchItem.shift();

        newSearchItem.map((e, i) => {
          if (e) {
            todos[i] = e.id;
            apenasUm = e.id.toString();
          }
          return apenasUm;
        });
        setSearchItem(newSearchItem);
        setChecked(newSearchItem);
        state?.onChange(state?.data.name, multiple ? todos : apenasUm);
      });
    } else {
      setSearchItem(value);
      setChecked(value);
      state?.onChange(state?.data.name, value);
    }
  };

  const customList = (title, items) => (
    <div style={{ marginBottom: '30px' }}>
      {/* <AutoComplete
        fullWidth
        floatingLabelText="Pesquise por código e/ou descrição (utilize ; para ambos)"
        searchText={searchItem || ''}
        filter={() => true}
        dataSource={itemsArray || []}
        dataSourceConfig={{ text: 'name', value: 'id' }}
        onUpdateInput={(v) => searchItems(v)}
        onNewRequest={(v, i) => setSearchItem(i === -1 ? '' : v.name)}
      /> */}
      <Autocomplete
        className="mb-3"
        multiple
        options={itemsArray || []}
        getOptionLabel={(option) => {
          if (typeof option === 'string') {
            return option;
          }
          if (option.descricao) {
            return option.descricao;
          }
          return option.descricao;
        }}
        loadingText="Carregando..."
        noOptionsText="Nenhum registro encontrado"
        onChange={(e, v) => {
          onChange(v);
          // console.log(a);
          // state.onChange('setor', v);
        }}
        onInputChange={(e, v) => {
          searchItems(v);
        }}
        fullWidth
        value={searchItem}
        renderInput={(params) => (
          <TextField
            {...params}
            size="medium"
            label="Pesquisar por"
            variant="outlined"
          />
        )}
      />
      <Card>
        {multiple && (
          <CardHeader
            className={classes.cardHeader}
            avatar={
              <Checkbox
                onClick={handleToggleAll(items)}
                checked={
                  numberOfChecked(items) === items.length && items.length !== 0
                }
                indeterminate={
                  numberOfChecked(items) !== items.length &&
                  numberOfChecked(items) !== 0
                }
                disabled={items.length === 0}
                inputProps={{ 'aria-label': 'all items selected' }}
              />
            }
            title={title}
            subheader={`${numberOfChecked(items)}/${items.length} selecionados`}
          />
        )}
        <Divider />
        <List className={classes.list} dense component="div" role="list">
          {itemsArray.map((value) => {
            const labelId = `transfer-list-all-item-${value.id}-label`;
            return (
              <ListItem
                key={value.id}
                role="listitem"
                button
                onClick={handleToggle(value)}>
                <ListItemIcon>
                  <Checkbox
                    checked={checked.indexOf(value) !== -1}
                    tabIndex={-1}
                    disableRipple
                    inputProps={{ 'aria-labelledby': labelId }}
                  />
                </ListItemIcon>
                <ListItemText
                  id={labelId}
                  primary={value.name || value.descricao}
                />
              </ListItem>
            );
          })}
          <ListItem />
        </List>
      </Card>
    </div>
  );

  return (
    <Grid
      container
      spacing={2}
      justify="center"
      alignItems="center"
      className={classes.root}>
      <Grid item>{customList(state?.data.label, left)}</Grid>
    </Grid>
  );
}
