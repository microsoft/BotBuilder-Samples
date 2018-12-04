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
    },
    plugins: [
        new CleanWebpackPlugin(['dist']),
        new webpack.NamedModulesPlugin(),
        new webpack.HotModuleReplacementPlugin(),
        new CopyWebpackPlugin([
            { from: path.resolve(__dirname, 'index.html'), to: '' }
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
};
