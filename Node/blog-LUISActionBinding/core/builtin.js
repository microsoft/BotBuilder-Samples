var BuiltInTypes = {
    Age:                    'builtin.age',
    Dimension:              'builtin.dimension',
    Email:                  'builtin.email',
    Money:                  'builtin.money',
    Number:                 'builtin.number',
    Ordinal:                'builtin.ordinal',
    Percentage:             'builtin.percentage',
    PhoneNumber:            'builtin.phonenumber',
    Temperature:            'builtin.temperature',
    Url:                    'builtin.url',
    DateTime: {
        Date:               'builtin.datetime.date',
        Time:               'builtin.datetime.time',
        Duration:           'builtin.datetime.duration',
        Set:                'builtin.datetime.set'
    },
    Encyclopedia: {
        Person:             'builtin.encyclopedia.people.person',
        Organization:       'builtin.encyclopedia.organization.organization',
        Event:              'builtin.encyclopedia.time.event'
    },
    Geography: {
        City:               'builtin.geography.city',
        Country:            'builtin.geography.country',
        PointOfInterest:    'builtin.geography.pointOfInterest'
    }
};

module.exports = BuiltInTypes;