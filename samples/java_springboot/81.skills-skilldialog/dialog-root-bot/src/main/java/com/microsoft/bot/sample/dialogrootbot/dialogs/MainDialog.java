package com.microsoft.bot.sample.dialogrootbot.dialogs;

import com.microsoft.bot.dialogs.BeginSkillDialogOptions;
import com.microsoft.bot.dialogs.ComponentDialog;
import com.microsoft.bot.dialogs.DialogContext;
import com.microsoft.bot.dialogs.DialogTurnResult;
import com.microsoft.bot.dialogs.SkillDialog;
import com.microsoft.bot.dialogs.SkillDialogOptions;
import com.microsoft.bot.dialogs.WaterfallDialog;
import com.microsoft.bot.dialogs.WaterfallStep;
import com.microsoft.bot.dialogs.WaterfallStepContext;
import com.microsoft.bot.dialogs.choices.Choice;
import com.microsoft.bot.dialogs.choices.FoundChoice;
import com.microsoft.bot.dialogs.prompts.ChoicePrompt;
import com.microsoft.bot.dialogs.prompts.PromptOptions;
import com.microsoft.bot.integration.Configuration;
import com.microsoft.bot.integration.SkillHttpClient;
import com.microsoft.bot.restclient.serializer.JacksonAdapter;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.concurrent.CompletableFuture;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.microsoft.bot.builder.ConversationState;
import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.builder.StatePropertyAccessor;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.builder.skills.BotFrameworkSkill;
import com.microsoft.bot.builder.skills.SkillConversationIdFactoryBase;
import com.microsoft.bot.connector.authentication.MicrosoftAppCredentials;
import com.microsoft.bot.sample.dialogrootbot.SkillsConfiguration;
import com.microsoft.bot.schema.Activity;
import com.microsoft.bot.schema.ActivityTypes;
import com.microsoft.bot.schema.InputHints;

import org.apache.commons.lang3.StringUtils;

public class MainDialog extends ComponentDialog {

    // Constants used for selecting actions on the skill.
    private final String SkillActionBookFlight = "BookFlight";
    private final String SkillActionBookFlightWithInputParameters = "BookFlight with input parameters";
    private final String SkillActionGetWeather = "GetWeather";
    private final String SkillActionMessage = "Message";

    public static final String ActiveSkillPropertyName =
        "com.microsoft.bot.sample.dialogrootbot.dialogs.MainDialog.ActiveSkillProperty";
    private final StatePropertyAccessor<BotFrameworkSkill> activeSkillProperty;
    private final String _selectedSkillKey =
        "com.microsoft.bot.sample.dialogrootbot.dialogs.MainDialog.SelectedSkillKey";
    private final SkillsConfiguration _skillsConfig;

    // Dependency injection uses this constructor to instantiate MainDialog.
    public MainDialog(
        ConversationState conversationState,
        SkillConversationIdFactoryBase conversationIdFactory,
        SkillHttpClient skillClient,
        SkillsConfiguration skillsConfig,
        Configuration configuration
    ) {
        super("MainDialog");
        String botId = configuration.getProperty(MicrosoftAppCredentials.MICROSOFTAPPID);
        if (StringUtils.isEmpty(botId)) {
            throw new IllegalArgumentException(
                String.format("%s is not in configuration", MicrosoftAppCredentials.MICROSOFTAPPID)
            );
        }

        if (skillsConfig == null) {
            throw new IllegalArgumentException("skillsConfig cannot be null");
        }

        if (skillClient == null) {
            throw new IllegalArgumentException("skillClient cannot be null");
        }

        if (conversationState == null) {
            throw new IllegalArgumentException("conversationState cannot be null");
        }

        _skillsConfig = skillsConfig;

        // Use helper method to add SkillDialog instances for the configured skills.
        addSkillDialogs(conversationState, conversationIdFactory, skillClient, skillsConfig, botId);

        // Add ChoicePrompt to render available skills.
        addDialog(new ChoicePrompt("SkillPrompt"));

        // Add ChoicePrompt to render skill actions.
        addDialog(new ChoicePrompt("SkillActionPrompt", (promptContext) -> {
            if (!promptContext.getRecognized().getSucceeded()) {
                // Assume the user wants to send a message if an item in the list is not
                // selected.
                FoundChoice foundChoice = new FoundChoice();
                foundChoice.setValue(SkillActionMessage);
                promptContext.getRecognized().setValue(foundChoice);
            }
            return CompletableFuture.completedFuture(true);
        }, ""));

        // Add main waterfall dialog for this bot.
        WaterfallStep[] waterfallSteps =
            {this::selectSkillStep, this::selectSkillActionStep, this::callSkillActionStep, this::finalStep};

        addDialog(new WaterfallDialog("WaterfallDialog", Arrays.asList(waterfallSteps)));

        // Create state property to track the active skill.
        activeSkillProperty = conversationState.createProperty(ActiveSkillPropertyName);

        // The initial child Dialog to run.
        setInitialDialogId("WaterfallDialog");
    }

    @Override
    protected CompletableFuture<DialogTurnResult> onContinueDialog(DialogContext innerDc) {
        // This instanceof an example on how to cancel a SkillDialog that instanceof
        // currently in progress from the parent bot.
        return activeSkillProperty.get(innerDc.getContext(), null).thenCompose(activeSkill -> {
            Activity activity = innerDc.getContext().getActivity();
            if (
                activeSkill != null && activity.getType().equals(ActivityTypes.MESSAGE)
                    && activity.getText().equals("abort")
            ) {
                // Cancel all dialogs when the user says abort.
                // The SkillDialog automatically sends an EndOfConversation message to the skill
                // to let the
                // skill know that it needs to end its current dialogs, too.
                return innerDc.cancelAllDialogs()
                    .thenCompose(
                        result -> innerDc
                            .replaceDialog(getInitialDialogId(), "Canceled! \n\n What skill would you like to call?")
                    );
            }

            return super.onContinueDialog(innerDc);
        });
    }

    // Render a prompt to select the skill to call.
    public CompletableFuture<DialogTurnResult> selectSkillStep(WaterfallStepContext stepContext) {
        String messageText = "What skill would you like to call?";
        // Create the PromptOptions from the skill configuration which contain the list
        // of configured skills.
        if (stepContext.getOptions() != null) {
            messageText = stepContext.getOptions().toString();
        }
        String repromptMessageText = "That was not a valid choice, please select a valid skill.";
        PromptOptions options = new PromptOptions();
        options.setPrompt(MessageFactory.text(messageText, messageText, InputHints.EXPECTING_INPUT));
        options
            .setRetryPrompt(MessageFactory.text(repromptMessageText, repromptMessageText, InputHints.EXPECTING_INPUT));

        List<Choice> choicesList = new ArrayList<Choice>();
        for (BotFrameworkSkill skill : _skillsConfig.getSkills().values()) {
            choicesList.add(new Choice(skill.getId()));
        }
        options.setChoices(choicesList);

        // Prompt the user to select a skill.
        return stepContext.prompt("SkillPrompt", options);
    }

    // Render a prompt to select the action for the skill.
    public CompletableFuture<DialogTurnResult> selectSkillActionStep(WaterfallStepContext stepContext) {
        // Get the skill info super. on the selected skill.
        String selectedSkillId = ((FoundChoice) stepContext.getResult()).getValue();
        BotFrameworkSkill selectedSkill = _skillsConfig.getSkills()
            .values()
            .stream()
            .filter(x -> x.getId().equals(selectedSkillId))
            .findFirst()
            .get();

        // Remember the skill selected by the user.
        stepContext.getValues().put(_selectedSkillKey, selectedSkill);

        // Create the PromptOptions with the actions supported by the selected skill.
        String messageText = String.format(
            "Select an action # to send to **%n** or just type in a " + "message and it will be forwarded to the skill",
            selectedSkill.getId()
        );
        PromptOptions options = new PromptOptions();
        options.setPrompt(MessageFactory.text(messageText, messageText, InputHints.EXPECTING_INPUT));
        options.setChoices(getSkillActions(selectedSkill));

        // Prompt the user to select a skill action.
        return stepContext.prompt("SkillActionPrompt", options);
    }

    // Starts the SkillDialog super. on the user's selections.
    public CompletableFuture<DialogTurnResult> callSkillActionStep(WaterfallStepContext stepContext) {
        BotFrameworkSkill selectedSkill = (BotFrameworkSkill) stepContext.getValues().get(_selectedSkillKey);

        Activity skillActivity;
        switch (selectedSkill.getId()) {
            case "DialogSkillBot":
                skillActivity = createDialogSkillBotActivity(
                    ((FoundChoice) stepContext.getResult()).getValue(),
                    stepContext.getContext()
                );
                break;

            // We can add other case statements here if we support more than one skill.
            default:
                throw new RuntimeException(String.format("Unknown target skill id: %s.", selectedSkill.getId()));
        }

        // Create the BeginSkillDialogOptions and assign the activity to send.
        BeginSkillDialogOptions skillDialogArgs = new BeginSkillDialogOptions();
        skillDialogArgs.setActivity(skillActivity);

        // Save active skill in state.
        activeSkillProperty.set(stepContext.getContext(), selectedSkill);

        // Start the skillDialog instance with the arguments.
        return stepContext.beginDialog(selectedSkill.getId(), skillDialogArgs);
    }

    // The SkillDialog has ended, render the results (if any) and restart
    // MainDialog.
    public CompletableFuture<DialogTurnResult> finalStep(WaterfallStepContext stepContext) {
        return activeSkillProperty.get(stepContext.getContext(), () -> null).thenCompose(activeSkill -> {
            if (stepContext.getResult() != null) {
                String jsonResult = "";
                try {
                    jsonResult =
                        new JacksonAdapter().serialize(stepContext.getResult()).replace("{", "").replace("}", "");
                } catch (IOException e) {
                    e.printStackTrace();
                }
                String message =
                    String.format("Skill \"%s\" invocation complete. Result: %s", activeSkill.getId(), jsonResult);
                stepContext.getContext().sendActivity(MessageFactory.text(message, message, InputHints.IGNORING_INPUT));
            }

            // Clear the skill selected by the user.
            stepContext.getValues().put(_selectedSkillKey, null);

            // Clear active skill in state.
            activeSkillProperty.delete(stepContext.getContext());

            // Restart the main dialog with a different message the second time around.
            return stepContext.replaceDialog(
                getInitialDialogId(),
                String.format("Done with \"%s\". \n\n What skill would you like to call?", activeSkill.getId())
            );
        });
        // Check if the skill returned any results and display them.
    }

    // Helper method that creates and adds SkillDialog instances for the configured
    // skills.
    private void addSkillDialogs(
        ConversationState conversationState,
        SkillConversationIdFactoryBase conversationIdFactory,
        SkillHttpClient skillClient,
        SkillsConfiguration skillsConfig,
        String botId
    ) {
        for (BotFrameworkSkill skillInfo : _skillsConfig.getSkills().values()) {
            // Create the dialog options.
            SkillDialogOptions skillDialogOptions = new SkillDialogOptions();
            skillDialogOptions.setBotId(botId);
            skillDialogOptions.setConversationIdFactory(conversationIdFactory);
            skillDialogOptions.setSkillClient(skillClient);
            skillDialogOptions.setSkillHostEndpoint(skillsConfig.getSkillHostEndpoint());
            skillDialogOptions.setConversationState(conversationState);
            skillDialogOptions.setSkill(skillInfo);
            // Add a SkillDialog for the selected skill.
            addDialog(new SkillDialog(skillDialogOptions, skillInfo.getId()));
        }
    }

    // Helper method to create Choice elements for the actions supported by the
    // skill.
    private List<Choice> getSkillActions(BotFrameworkSkill skill) {
        // Note: the bot would probably render this by reading the skill manifest.
        // We are just using hardcoded skill actions here for simplicity.

        List<Choice> choices = new ArrayList<Choice>();
        switch (skill.getId()) {
            case "DialogSkillBot":
                choices.add(new Choice(SkillActionBookFlight));
                choices.add(new Choice(SkillActionBookFlightWithInputParameters));
                choices.add(new Choice(SkillActionGetWeather));
                break;
        }

        return choices;
    }

    // Helper method to create the activity to be sent to the DialogSkillBot using
    // selected type and values.
    private Activity createDialogSkillBotActivity(String selectedOption, TurnContext turnContext) {

        // Note: in a real bot, the dialogArgs will be created dynamically super. on the
        // conversation
        // and what each action requires; here we hardcode the values to make things
        // simpler.
        ObjectMapper mapper = new ObjectMapper();
        Activity activity = null;

        // Just forward the message activity to the skill with whatever the user said.
        if (selectedOption.equalsIgnoreCase(SkillActionMessage)) {
            // Note message activities also support input parameters but we are not using
            // them in this example.
            // Return a deep clone of the activity so we don't risk altering the original
            // one
            activity = Activity.clone(turnContext.getActivity());
        }

        // Send an event activity to the skill with "BookFlight" in the name.
        if (selectedOption.equalsIgnoreCase(SkillActionBookFlight)) {
            activity = Activity.createEventActivity();
            activity.setName(SkillActionBookFlight);
        }

        // Send an event activity to the skill with "BookFlight" in the name and some
        // testing values.
        if (selectedOption.equalsIgnoreCase(SkillActionBookFlightWithInputParameters)) {
            activity = Activity.createEventActivity();
            activity.setName(SkillActionBookFlight);
            try {
                activity.setValue(
                    mapper.readValue("{ \"origin\": \"New York\", \"destination\": \"Seattle\"}", Object.class)
                );
            } catch (JsonProcessingException e) {
                e.printStackTrace();
            }
        }

        // Send an event activity to the skill with "GetWeather" in the name and some
        // testing values.
        if (selectedOption.equalsIgnoreCase(SkillActionGetWeather)) {
            activity = Activity.createEventActivity();
            activity.setName(SkillActionGetWeather);
            try {
                activity
                    .setValue(mapper.readValue("{ \"latitude\": 47.614891, \"longitude\": -122.195801}", Object.class));
            } catch (JsonProcessingException e) {
                e.printStackTrace();
            }
        }

        if (activity == null) {
            throw new RuntimeException(String.format("Unable to create a skill activity for \"%s\".", selectedOption));
        }

        // We are manually creating the activity to send to the skill; ensure we add the
        // ChannelData and Properties
        // from the original activity so the skill gets them.
        // Note: this instanceof not necessary if we are just forwarding the current
        // activity from context.
        activity.setChannelData(turnContext.getActivity().getChannelData());
        for (String key : turnContext.getActivity().getProperties().keySet()) {
            activity.setProperties(key, turnContext.getActivity().getProperties().get(key));
        }

        return activity;
    }
}
