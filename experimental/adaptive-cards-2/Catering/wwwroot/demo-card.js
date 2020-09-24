window.App = window.App || {};

const sampleData = {
  title: 'Publish Adaptive Card Schema',
  description:
    'Now that we have defined the main rules and features of the format, we need to produce a schema and publish it to GitHub. The schema will be the starting point of our reference documentation.',
  creator: {
    name: 'Matt Hidinger',
    profileImage: 'https://pbs.twimg.com/profile_images/3647943215/d7f12830b3c17a5a9e4afcc370e3a37e_400x400.jpeg'
  },
  createdUtc: '2017-02-14T06:08:39Z',
  viewUrl: 'https://adaptivecards.io',
  properties: [
    { key: 'Board', value: 'Adaptive Cards' },
    { key: 'List', value: 'Backlog' },
    { key: 'Assigned to', value: 'Matt Hidinger' },
    { key: 'Due date', value: 'Not set' }
  ]
};

const sampleCard = {
  type: 'AdaptiveCard',
  appId: 'abcd',
  body: [
    {
      type: 'TextBlock',
      size: 'Medium',
      weight: 'Bolder',
      text: '{title}'
    },
    {
      type: 'ColumnSet',
      columns: [
        {
          type: 'Column',
          items: [
            {
              type: 'Image',
              style: 'Person',
              url: '{creator.profileImage}',
              size: 'Small'
            }
          ],
          width: 'auto'
        },
        {
          type: 'Column',
          items: [
            {
              type: 'TextBlock',
              weight: 'Bolder',
              text: '{creator.name}',
              wrap: true
            },
            {
              type: 'TextBlock',
              spacing: 'None',
              text: 'Created {{DATE({createdUtc},SHORT)}}',
              isSubtle: true,
              wrap: true
            }
          ],
          width: 'stretch'
        }
      ]
    },
    {
      type: 'TextBlock',
      text: '{description}',
      wrap: true
    },
    {
      type: 'FactSet',
      facts: [
        {
          $data: '{properties}',
          title: '{key}:',
          value: '{value}'
        }
      ]
    }
  ],
  actions: [
    {
      type: 'Action.Execute',
      verb: 'remoteSucceedReturnCard',
      title: 'Succeed in 3 attempts with card'
    },
    {
      type: 'Action.Execute',
      verb: 'remoteSucceedReturnString',
      title: 'Succeed with string'
    },
    {
      type: 'Action.Execute',
      verb: 'remoteFailedUnrecoverable',
      title: 'Fail'
    },
    {
      type: 'Action.Execute',
      verb: 'remoteFailedUnauthenticated',
      title: 'Fail with auth'
    },
    {
      type: 'Action.Execute',
      verb: 'localException',
      title: 'Fail with exception'
    }
  ],
  $schema: 'http://adaptivecards.io/schemas/adaptive-card.json',
  version: '1.0'
};

const sampleRefreshCard = {
  type: 'AdaptiveCard',
  appId: 'abcd',
  body: [
    {
      type: 'TextBlock',
      text: 'This card has been refreshed',
      size: 'ExtraLarge',
      wrap: true
    },
    {
      type: 'TextBlock',
      text: 'The response to the Submit action included a new card payload.',
      wrap: true
    }
  ],
  $schema: 'http://adaptivecards.io/schemas/adaptive-card.json',
  version: '1.0'
};

window.App.sampleData = sampleData;
window.App.sampleCard = sampleCard;
window.App.sampleRefreshCard = sampleRefreshCard;
