'use strict';

window.Redux || (window.Redux = {});

window.Redux.compose = function compose(...funcs) {
  if (funcs.length === 0) {
    // infer the argument type so it is usable in inference down the line
    return arg => arg;
  }

  if (funcs.length === 1) {
    return funcs[0];
  }

  return funcs.reduce((a, b) => (...args) => a(b(...args)));
};
