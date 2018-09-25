import { TemplateManager } from "../templateManager/templateManager";

export class MainResponses extends TemplateManager {
    // Constants
    public static readonly Cancelled: string = 'cancelled';
    public static readonly Completed: string = 'completed';
    public static readonly Confused: string = 'confused';
    public static readonly Greeting: string = 'greeting';
    public static readonly Help: string = 'help';
    public static readonly Intro: string = 'intro';

    constructor() {
        super();
        
    }
}