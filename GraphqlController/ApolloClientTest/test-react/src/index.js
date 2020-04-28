import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import App from './App';
import * as serviceWorker from './serviceWorker';
import ApolloClient, { ApolloLink } from 'apollo-client';
import { createPersistedQueryLink } from "apollo-link-persisted-queries";
import { createHttpLink } from 'apollo-link-http';
import { InMemoryCache } from "apollo-cache-inmemory";

// use this with Apollo Client
const link = createPersistedQueryLink().concat(createHttpLink({ uri: "https://localhost:44399/graphql/root" }));
const client = new ApolloClient({
  cache: new InMemoryCache(),
  link: link,
  connectToDevTools: true
});

ReactDOM.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>,
  document.getElementById('root')
);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
