import { createElement } from 'react';

// Helper function to hoist a component as HOC with corresponding props from a specified React context.
export default (context, selector = state => state) => component => props =>
  createElement(
    context.Consumer,
    {},
    state => createElement(
      component,
      {
        ...props,
        ...selector(state)
      }
    )
  )
