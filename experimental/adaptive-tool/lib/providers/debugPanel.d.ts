/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import * as vscode from 'vscode';
export declare function activate(context: vscode.ExtensionContext): void;
/**
 * VebView for LG debugger
 *
 * @export
 * @class LGDebugPanel
 */
export declare class LGDebugPanel {
    /**
     * Track the currently panel. Only allow a single panel to exist at a time.
     */
    static currentPanel: LGDebugPanel | undefined;
    static readonly viewType: string;
    private readonly _panel;
    private readonly _extensionPath;
    private _disposables;
    /**
     * Command lgLivingTest.start runner
     *
     * @static
     * @returns
     * @memberof LGDebugPanel
     */
    static createOrShow(extensionPath: string): void;
    dispose(): void;
    private constructor();
    private _update;
    private _getHtmlForWebview;
}
//# sourceMappingURL=debugPanel.d.ts.map