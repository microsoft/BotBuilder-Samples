const path = require('path');
const CleanWebpackPlugin = require('clean-webpack-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const webpack = require('webpack');

module.exports = {
    entry: './src/app.js',
    devtool: 'source-map',
    devServer: {
        contentBase: './dist',
        hot: true
    },
<<<<<<< HEAD
    module: {
      rules: [
          {
              test: /\.css$/,
              use: [ 'style-loader', 'css-loader' ]
          }
      ]
=======
    mode: 'development',
    module: {
        rules: [
            {
                test: /\.(jsx?)$/,
                exclude: [/node_modules/],
                use: {
                    loader: 'babel-loader',
                    'options': {
                        'ignore': ['**/*.spec.ts']
                    }
                }
            },
            {
                test: /\.css$/,
                use: ['style-loader', 'css-loader']
            }
        ]
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
    },
    plugins: [
        new CleanWebpackPlugin(['dist']),
        new webpack.NamedModulesPlugin(),
        new webpack.HotModuleReplacementPlugin(),
        new CopyWebpackPlugin([
<<<<<<< HEAD
            {from: path.resolve(__dirname, 'index.html'), to: ''},
=======
            { from: path.resolve(__dirname, 'index.html'), to: '' }
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
        ])
    ],
    output: {
        filename: 'app.js',
        path: path.resolve(__dirname, 'dist')
    },
    node: {
        fs: 'empty',
        net: 'empty',
        tls: 'empty'
    }
<<<<<<< HEAD
};
=======
};
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
