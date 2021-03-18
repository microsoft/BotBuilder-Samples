# School Navigator Sample (2)



The following sample is identical to the sample in [../01.school-skill-navigator](../01.school-skill-navigator/README.md) but migrated to the new Composer runtime. 

## Migration Steps

The migration was performed manually as follows:

1. Create new empty bot
2. Copy all declarative assets from the old bot to the new bot
3. Install the Orchestrator package (see Composer documentation)
4. Select another recognizer, then switch back the Orchestrator to trigger settings update

The declarative assets that were copies over area as follows:

- Folder: dialogs
- Folder: knowledge-base
- Folder: language-generation
- Folder: language-understanding
- File: schoolnavigatorbot.dialog
  - File: *remove* original root dialog (emptyBot.dialog)