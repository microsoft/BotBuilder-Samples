from flask import Flask, request, jsonify
import requests
import json
from datetime import datetime

app = Flask(__name__)

# OpenWeatherMap API
api_key = 'f0dc5cc10d23b04303ede6a45d9628bb'

def get_weather(city):    
    # API url
    url = f'http://api.openweathermap.org/data/2.5/weather?q={city}&appid={api_key}'

    # send request to the API
    response = requests.get(url)

    if response.status_code == 200:
        data = response.json()

        # retrieve needed weather data from api response
        weather_data = {
            'temp': (data['main']['temp'] - 273) * 9 / 5 + 32,
            'name': city,
            'dt': data['dt'],
            'description': data['weather'][0]['description']
        }

        return weather_data
    
    else:
        return 'Weather information unavailable'


def create_card(weather_data):    
    # open card template to be updated
    with app.open_resource('resources/WeatherCompactCard.json') as template_file:
        updated_card = json.load(template_file)
    
    # date object created to format date and time correctly
    date_object = datetime.fromtimestamp(weather_data['dt']).strftime('%m/%d/%Y %H:%M')
    

    # replace and update card info to be sent
    updated_card['body'][0]['text'] = weather_data['name'].title()
    updated_card['body'][1]['text'] = date_object
    updated_card['body'][2]['columns'][1]['items'][0]['text'] = "{}".format(int(weather_data['temp']))
    updated_card['body'][2]['columns'][0]['items'][0]['text'] = weather_data['description']

    return updated_card


@app.route('/new_weather_card', methods=['GET'])
def new_weather_card():
    # get city name from user input in adaptiveCardsBot.js file
    city = request.args.get('city')

    # get weather info from API for selected city
    weather_info = get_weather(city)

    # create weather card with weather information
    weather_card = create_card(weather_info)

    return jsonify(weather_card)
    

if __name__ == '__main__':
    app.run(debug=True)