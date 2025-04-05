using ArmyUnline.Scripts.Enums;
using Godot;
using Godot.Collections;
using Scripts.Enums;

namespace DialogueManagerRuntime
{
    public partial class DialogueBalloon : CanvasLayer
    {
        ////////////////////////////////
        //      外部控制變量宣告       //
        ///////////////////////////////
        [Export] public string NextAction = "ui_accept";//下一步
        [Export] public string SkipAction = "ui_cancel";//跳過
        [Export] public string CharacterPortraitsPath = "res://Images/Characters";//角色頭像ルート
        [Export] public string BackgroundPath = "res://Images/SenceBG";//背景画像ルート
        [Export] public string DefaultMood = "normal"; // 默认心情

        public TalkSceneState talkSceneState;//今の対話

        ////////////////////////////////
        //      內部控制變量宣告       //
        ///////////////////////////////
        Control balloon;//對話框
        Panel characterNamePanel;//角色名稱面板
        RichTextLabel characterLabel;
        RichTextLabel dialogueLabel;
        Sprite2D CharaterHeadPic;
        Sprite2D BackGraphic;
        VBoxContainer responsesMenu;
        

        Resource resource;
        Array<Variant> temporaryGameStates = new Array<Variant>();
        bool isWaitingForInput = false;
        bool willHideBalloon = false;

        DialogueLine currentDialogueLine;
        DialogueLine DialogueLine
        {
            get => currentDialogueLine;
            set
            {
                isWaitingForInput = false;
                balloon.FocusMode = Control.FocusModeEnum.All;
                balloon.GrabFocus();

                if (value == null)
                {
                    QueueFree();
                    return;
                }

                currentDialogueLine = value;
                UpdateDialogue();
            }
        }

        //////////////////////////////////
        //     　インストール備え　　    //
        /////////////////////////////////
        public override void _Ready()
        {
            balloon = GetNode<Control>("%Balloon");
            characterNamePanel = GetNode<Panel>("%CharacterNamePanel");
            characterLabel = GetNode<RichTextLabel>("%CharacterLabel");
            dialogueLabel = GetNode<RichTextLabel>("%DialogueLabel");
            responsesMenu = GetNode<VBoxContainer>("%ResponsesMenu");
            CharaterHeadPic = GetNode<Sprite2D>("%CharaterHeadPic");
            BackGraphic = GetNode<Sprite2D>("%BackGraphic");
            talkSceneState=new TalkSceneState();

            balloon.Hide();

            balloon.GuiInput += (@event) =>
            {
                if ((bool)dialogueLabel.Get("is_typing"))
                {
                    bool mouseWasClicked = @event is InputEventMouseButton && (@event as InputEventMouseButton).ButtonIndex == MouseButton.Left && @event.IsPressed();
                    bool skipButtonWasPressed = @event.IsActionPressed(SkipAction);
                    if (mouseWasClicked || skipButtonWasPressed)
                    {
                        GetViewport().SetInputAsHandled();
                        dialogueLabel.Call("skip_typing");
                        return;
                    }
                }

                if (!isWaitingForInput) return;
                if (currentDialogueLine.Responses.Count > 0) return;

                GetViewport().SetInputAsHandled();

                if (@event is InputEventMouseButton && @event.IsPressed() && (@event as InputEventMouseButton).ButtonIndex == MouseButton.Left)
                {
                    Next(currentDialogueLine.NextId);
                }
                else if (@event.IsActionPressed(NextAction) && GetViewport().GuiGetFocusOwner() == balloon)
                {
                    Next(currentDialogueLine.NextId);
                }
            };

            if (string.IsNullOrEmpty((string)responsesMenu.Get("next_action")))
            {
                responsesMenu.Set("next_action", NextAction);
            }
            responsesMenu.Connect("response_selected", Callable.From((DialogueResponse response) =>
            {
                Next(response.NextId);
            }));

            DialogueManager.Mutated += OnMutated;
        }
        ////////////////////////////////
        //      Destroy時の処理       //
        ///////////////////////////////
        public override void _ExitTree()
        {
            DialogueManager.Mutated -= OnMutated;
        }


        public override void _UnhandledInput(InputEvent @event)
        {
            // Only the balloon is allowed to handle input while it's showing
            GetViewport().SetInputAsHandled();
        }


        public override async void _Notification(int what)
        {
            // Detect a change of locale and update the current dialogue line to show the new language
            if (what == NotificationTranslationChanged)
            {
                float visibleRatio = dialogueLabel.VisibleRatio;
                DialogueLine = await DialogueManager.GetNextDialogueLine(resource, DialogueLine.Id, temporaryGameStates);
                if (visibleRatio < 1.0f)
                {
                    dialogueLabel.Call("skip_typing");
                }
            }
        }


        public async void Start(Resource dialogueResource, string title, Array<Variant> extraGameStates = null)
        {
            temporaryGameStates = extraGameStates ?? new Array<Variant>();
            isWaitingForInput = false;
            resource = dialogueResource;

            DialogueLine = await DialogueManager.GetNextDialogueLine(resource, title, temporaryGameStates);
        }
        ////////////////////////////////
        //      次のダイアログ 　      //
        ///////////////////////////////
        public async void Next(string nextId)
        {
            DialogueLine = await DialogueManager.GetNextDialogueLine(resource, nextId, temporaryGameStates);
        }

        #region Helpers



        ////////////////////////////////
        //      背景絵の更新　　       //
        ///////////////////////////////
        private void UpdateBackGrafic(string backGroundPic)
        {
            if(GD.Load<Texture2D>(BackgroundPath+"/"+backGroundPic+".png")!=null)
            {
                BackGraphic.Texture = GD.Load<Texture2D>(BackgroundPath+"/"+backGroundPic+".png");
            }
            else
            {
                GD.PrintErr($"无法加载背景图片: {BackgroundPath+"/"+backGroundPic+".png"}");
            }
            
        }

        ////////////////////////////////
        //      角色頭像の更新　　     //
        ///////////////////////////////
        private void UpdateCharacterPortrait(string characterPicName)
        {
            if (string.IsNullOrEmpty(characterPicName)||characterPicName.Equals("楠"))//ダン
            {
                GD.Print("没有角色");
                CharaterHeadPic.Visible = false;
                return;
            }

            //ムードの取得
            string mood =currentDialogueLine.GetTagValue("mood");
            //ポートレートのパス
            string portraitPath = $"{CharacterPortraitsPath}/{characterPicName}/{characterPicName}_{mood}.png";
            //ポートレートのテクスチャ
            var texture = GD.Load<Texture2D>(portraitPath);
            //ポートレートのテクスチャが存在する場合
            if (texture != null)
            {
                CharaterHeadPic.Texture = texture;
                CharaterHeadPic.Visible = true;
            }
            else
            {
                // ない場合
                if (mood != DefaultMood)
                {
                    portraitPath = $"{CharacterPortraitsPath}/{characterPicName}/{characterPicName}_{DefaultMood}.png";
                    texture = GD.Load<Texture2D>(portraitPath);
                    if (texture != null)
                    {
                        CharaterHeadPic.Texture = texture;
                        CharaterHeadPic.Visible = true;
                        return;
                    }
                }
                
                GD.PrintErr($"无法加载角色头像: {portraitPath}");
                CharaterHeadPic.Visible = false;
            }
        }

        ////////////////////////////////
        //     　　 對話更新　　       //
        ///////////////////////////////
        private async void UpdateDialogue()
        {
            if (!IsNodeReady())
            {
                await ToSignal(this, SignalName.Ready);
            }
            #region キャラクター更新

            //character name 分成圖片角色與顯示名稱
            string characterPicName ="";
            //如果有圖片角色與顯示名稱,則分開兩者
            if(currentDialogueLine.Character.Split('|').Length>1)
            {
                characterPicName= currentDialogueLine.Character.Split('|')[0];
                //更新顯示名稱 
                currentDialogueLine.Character = currentDialogueLine.Character.Split('|')[1];
            }
            //ない場合はただ名稱を表示
            talkSceneState.characterName= currentDialogueLine.Character;
            // 更新キャラクターのポートレート
                UpdateCharacterPortrait(characterPicName);
            // 更新キャラクター名稱
            characterNamePanel.Visible = !string.IsNullOrEmpty(currentDialogueLine.Character);
            characterLabel.Text = Tr(currentDialogueLine.Character, "dialogue");
            #endregion

            #region Tag更新
            foreach (string tag in currentDialogueLine.Tags)
            {
                string tagName = tag.Split('=')[0];
                string tagValue = tag.Split('=')[1];
                switch(tagName)
                {
                    case "Background":
                        //背景更新
                        UpdateBackGrafic(tagValue);
                        break;
                    case "BGM":
                        //背景音樂更新
                        AudioSystem.audioSystemInstance.PlayAudio(AudioType.BGM,tagValue,true);
                        break;
                    case "SFX":
                        //音效更新
                        AudioSystem.audioSystemInstance.PlayAudio(AudioType.SFX,tagValue,false);
                        break;
                    case "HSE":
                        //Hシーン背景音樂更新
                        AudioSystem.audioSystemInstance.PlayAudio(AudioType.HSE,tagValue,true);
                        break;
                    case "Voice":
                        //角色語音更新
                        AudioSystem.audioSystemInstance.PlayAudio(AudioType.Voice,tagValue,false);
                        break;
                }
            }
            #endregion

            // 更新ダイアログ
            dialogueLabel.Hide();
            dialogueLabel.Set("dialogue_line", currentDialogueLine);

            // 更新選択肢
            responsesMenu.Hide();
            responsesMenu.Set("responses", currentDialogueLine.Responses);

            // テキストを出力
            balloon.Show();
            willHideBalloon = false;
            dialogueLabel.Show();
            if (!string.IsNullOrEmpty(currentDialogueLine.Text))
            {
                dialogueLabel.Call("type_out");
                await ToSignal(dialogueLabel, "finished_typing");
            }

            // 入力を待つ
            if (currentDialogueLine.Responses.Count > 0)
            {
                balloon.FocusMode = Control.FocusModeEnum.None;
                responsesMenu.Show();
            }
            else if (!string.IsNullOrEmpty(currentDialogueLine.Time))
            {
                float time = 0f;
                if (!float.TryParse(currentDialogueLine.Time, out time))
                {
                    time = currentDialogueLine.Text.Length * 0.02f;
                }
                await ToSignal(GetTree().CreateTimer(time), "timeout");
                Next(currentDialogueLine.NextId);
            }
            else
            {
                isWaitingForInput = true;
                balloon.FocusMode = Control.FocusModeEnum.All;
                balloon.GrabFocus();
            }
        }


        #endregion


        #region signals
        private void OnDialogueEnd()
        {
            
        }

        private void OnMutated(Dictionary _mutation)
        {
            isWaitingForInput = false;
            willHideBalloon = true;
            GetTree().CreateTimer(0.1f).Timeout += () =>
            {
                if (willHideBalloon)
                {
                    willHideBalloon = false;
                    balloon.Hide();
                }
            };
        }


        #endregion
    }
}
