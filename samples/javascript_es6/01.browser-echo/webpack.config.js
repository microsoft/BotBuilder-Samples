const { join, resolve } = require('path');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const { HotModuleReplacementPlugin, NamedModulesPlugin } = require('webpack');

module.exports = {
    entry: './src/app.ts',
    devtool: 'source-map',
    devServer: {
        contentBase: './dist',
        hot: true
    },
    mode: 'development',
    module: {
        rules: [
            {
                test: /\.[jt]s$/,
                include: [
                    join(__dirname, 'src'),
                    join(__dirname, 'node_modules/botbuilder-core/lib'),
                ],
                use: ['babel-loader']
            },
            {
                test: /\.css$/,
                use: ['style-loader', 'css-loader']
            }
        ]
    },
    plugins: [
        new CleanWebpackPlugin(),
        new NamedModulesPlugin(),
        new HotModuleReplacementPlugin(),
        new CopyWebpackPlugin([
            { from: resolve(__dirname, 'index.html'), to: '' }
        ])
    ],
    resolve: {
        extensions: ['.css', '.js', '.ts']
    },
    output: {
        filename: 'app.js',
        path: resolve(__dirname, 'dist')
    },
    node: {
        fs: 'empty',
        net: 'empty',
        tls: 'empty'
    }
};
