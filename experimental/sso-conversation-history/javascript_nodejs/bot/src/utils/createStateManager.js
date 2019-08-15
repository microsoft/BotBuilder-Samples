module.exports = function createStateBag(state, accessor, context, defaultState = {}) {
  return {
    getState() {
      return accessor.get(context, defaultState);
    },
    saveChanges() {
      return state.saveChanges(context);
    }
  };
};
