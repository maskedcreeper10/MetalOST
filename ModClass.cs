using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Language;
using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Video;
using WavLib;



namespace MetalOST
{
    public class MetalOST : Mod, IMenuMod, IGlobalSettings<GlobalSettings>
    {
        internal static MetalOST instance;

        //Variables

        public bool ToggleButtonInsideMenu => true;
        private readonly Assembly assembly = Assembly.GetExecutingAssembly();
        private Dictionary<string, AudioClip> AudioCache = new Dictionary<string, AudioClip>();
        private readonly List<string> BossList = [
            "S82-122 Grimm Epic Layer",
            "Hollow Shade Music", 
            "Boss Battle 1", 
            "gg_eternal_ordeal_music",
            "GG10-53 Vessel", 
            "GG3_part_A", 
            "GG3_part_B2",
            "GG3_part_B3", 
            "GG4-31 Sad 1 Main", 
            "GG4-31 Sad 2 Timp and Snare", 
            "GG4-31 Sad 3 High Theme", 
            "GG4-31 Sad 4 High Short StringsAndBrass", 
            "GG7 Mantis Lords-51", 
            "GG8 Hornet-155", 
            "Hive Knight v2-86", 
            "S18 Enemy Battle-02 LOOP", 
            "S34-43 MAGE BOSS", 
            "S45 HORNET-110", 
            "S47-85 Mantis Lords",
            "S48-66 Dung Defender", 
            "S49B-23 MIMIC SPIDER", 
            "S52-99 BROKEN WANDERER", 
            "S53-30 Dream Battle", 
            "S55-13 Mage Under Glass", 
            "S57 COLOSSEUM INTENSITY 1", 
            "S57 COLOSSEUM INTENSITY 2", 
            "S57 COLOSSEUM INTENSITY 3", 
            "S57 COLOSSEUM INTENSITY 4", 
            "S57 COLOSSEUM INTENSITY 5", 
            "S57 COLOSSEUM INTENSITY 6", 
            "S57 COLOSSEUM STING", 
            "S59-55 Final Stage 1", 
            "S59-55 Final Stage 2", 
            "S59-55 Final Stage 3", 
            "S59-55 Final Stage 4", 
            "S59-55 Final Stage 5", 
            "S61-161 Suspence 1", 
            "S61-161 Suspence 2", 
            "S61-161 Suspence 3", 
            "S61-161 Suspence 4", 
            "S61-161 Suspence 5", 
            "S61-216 Hollow Knight", 
            "S72-140 FINAL FINAL BOSS", 
            "S76b-458 White Defender", 
            "S81-195 Grey Prince Zote", 
            "S82-115 Grimm", 
            "S87-168 Nightmare Grimm Optional Ending", 
            "S87-168 Nightmare Grimm", 
            "Silent" 
            ];
        private readonly List<string> EnviromentList = [
            "Hollow Shade Music", 
            "S89 Accordion Dirtmouth-16", 
            "S19 Optional Drone", 
            "Dirtmouth 1", 
            "GG5-04_atrium", 
            "RESTING GROUNDS S51-14", 
            "RoyalTheme_QueenNew", 
            "S19 Action", 
            "S19 Crossroads Bass", 
            "S19 Crossroads Main",
            "S19 Infected", 
            "S19 Shaman", 
            "S23-11 INSIDE LOOP", 
            "S23-11 OUTSIDE LOOP", 
            "S23-19 ACTION LOOP", 
            "S25 Fungal Wastes BASS Mantis", 
            "S25 Fungal Wastes BASS Pizz", 
            "S25 Fungal Wastes MAIN", 
            "S26 Crystal ACTION", 
            "S26 Crystal BASS", 
            "S26 Crystal MAIN", 
            "S30 White Palace", 
            "S31 Waterways", 
            "S54-12 Waterways Action", 
            "S32_Deepnest", 
            "S36 04.10.2016-04 Dream",
            "S39 Kingdoms Edge-23", 
            "S41-33 Strings and Choir",
            "S42 Queens Garden-30 ACTION", 
            "S42 Queens Garden-30 MAIN", 
            "S42 Queens Garden-30 SUB", 
            "S44-01 New Dream Cello", 
            "S5 Fog Canyon v2", 
            "S5 Green Path Action", 
            "S5 Green Path Bass", 
            "S5 Green Path Main", 
            "S59-55 Final Stage 1", 
            "S59-55 Final Stage 2", 
            "S59-55 Final Stage 3", 
            "S59-55 Final Stage 4", 
            "S59-55 Final Stage 5", 
            "S56-03 Optional NPC Room", 
            "s63-20 SOUL SOCIETY", 
            "S68-04 HIVE", 
            "Safety", 
            "Silent", 
            "Title" 
            ];
        private readonly List<string> PoPRooms = [
            "WHITE_PALACE_18",
            "WHITE_PALACE_17",
            "WHITE_PALACE_19",
            "WHITE_PALACE_20",
            ];


        // not all songs have been made, so some will be replaced with one that has been made.
        private readonly Dictionary<string, string> Exceptions = new Dictionary<string, string>
        {
            {"S19 Optional Drone", "Silent"},
            {"S82-122 Grimm Epic Layer", "Silent"},
            {"S89 Accordion Dirtmouth-16", "Dirtmouth 1"}
        };
        private readonly Dictionary<string, List<string>> customDialogue = new Dictionary<string, List<string>>
        {
            //KEY, [ORIG, NEW}

            //Misc
            {"PLAQUE_WARN", ["To witness secrets sealed, one must endure the harshest punishment.", "To witness melodies tuned, one must complete the fractured harmony."]},
            {"KING_FINAL_WORDS", ["...Soul of Wyrm. Soul of Root. Heart of Void...", "...Soul of Wyrm. Soul of Root. Heart of Void... <br> ...Heart of Metal..."] },
            {"KNIGHT_STATUE_1", ["Not bug, nor beast, nor god.", "Not bug, nor beast, nor god... <page> But a vessel, with a Metal Heart."] },
            {"KNIGHT_STATUE_2", ["Void given form.", "Void given form due to Metal Heart."] },
            {"KNIGHT_STATUE_3", ["Void given focus.", "Void given Metal focus"] },
            {"MARISSA_TALK", ["Welcome to my stage little one. I am Marissa, a songstress of some renown, though given the sorry state of this place, you may find it hard to believe. <page>Huge crowds once flocked to hear me sing, then something changed. The audience, once so enrapt, began to leave. I continued to sing yet my voice fell silent upon their ears.<page>Perhaps you'd care to listen to me sing? You'll be the first in an age to hear it.", "Welcome to my stage little one. I am Marissa, a songstress of some renown, though given the sorry state of this place I found, you may find it hard to believe. <page>Huge crowds once flocked to hear me sing, then something changed. The audience, once so enrapt, began to leave. I continued to sing yet my voice fell silent upon their ears.<page>So I decided to start singing along to this weird guitar riff I was hearing in my head. <page> Perhaps you'd care to listen to me sing? You'll be the first in an age to hear it."] },
            //God journals
            {"NOTE_NAILMASTERS", ["\"Gods by toil and nail bound,<br>Brothers sworn to guard the weak,<br>Masters of the sacred ground,<br>Help Us find the God We seek!\"<br>- Prayer to the Masters", "\"By nail and steel you are tried, <br>Brothers divided, yet amplified. <br>Masters, through the riffs you speak, <br>Through torn chords and drums that peak,<br>Take us through the feedback's shriek, <br>And lead us to that motif we seek!\" <br>  - Ode to the Masters"] },
            {"NOTE_PAINTMASTER", ["\"O God inspired, master of arts,<br>Whose works shall eternal endure,<br>Peer beyond Our minds and hearts,<br>Reveal to Us the God most pure!\"<br>- Prayer to the Artist", "\"By brush and chord your colors sing,<br>Shades that bleed like burning string.<br>Artist, through your strokes you speak,<br>With splattered tones the shadows leak.<br>Take us where the hues consume,<br>And lead us to the final tune.\"<br>  - Ode to the Artist"] },
            {"NOTE_SAGE_SLY", ["\"Sagely God of the cunning and bold,<br>Sharpen Our nails and show Us the odds,<br>O greatest of masters, We wish to behold,<br>That one still greater, the God of Gods!\"<br>- Prayer to the Sage", "\"Small in frame, yet might that clings,<br>Geo's heartbeat shapes your swings.<br>Great Nailsage, your voice commands,<br>Notes that strike like unseen hands.<br>Guide us past the silent curse,<br>To find and claim that final verse.\"<br>  - Ode to the Sage"] },
            {"NOTE_PURE_VESSEL", ["\"Deepest silence in holy shell,<br>Given nail and named a Knight,<br>Bound by chain and egg and spell,<br>Hear Our plea! Reveal thy Light!\"<br>- Prayer to the Vessel", "\"Forged in void where silence sings,<br>Bound by fate on tethered string.<br>Vessel, mute, yet paths you leave,<br>Bearing truths we can't conceive.<br>Carry us through time's suspense,<br>Toward the light of holy cadence!\"<br>  - Ode to the Vessel"] },
            {"NOTE_GODSEEKER_MASK", ["\"Gods of Thunder, Gods of Rain! Why forsake thy servants? Will Our minds be left suffering, to ache alone? What God remains to deliver Us from this woeful silence?\"<br>- Lament of the Godseekers", "\"O voices lost beyond the storm,<br>Why leave our hearts so weak, forlorn?<br>The skies grow mute, the rains decay,<br>No song remains to light the way.<br>Shall silence bind our every sound,<br>And pull us deep where none are found?<br>We reach for song to heal our breath,<br>But silence sings the hymn of death.\"<br>  - Requiem of the Godseekers"] },
            {"NOTE_FLAMEBEARER_SMALL", ["\"Shadows dream of endless fire,<br>Flames devour and embers swoop,<br>One will light the Nightmare Lantern,<br>Call and serve in Grimm's dread Troupe.\"<br>- 'The Grimm Troupe'", "\"Shadows drum in burning fire,<br>Flames strike chords of dark desire.<br>One will join the crimson group,<br>And play for Grimm's eternal troupe.\"<br>  -The Grimm Troupe"] },
            {"NOTE_FLAMEBEARER_MED", ["\"A spark of red lights darkest dream,<br>Scarlet nightmares bright and wild,<br>Visions dance and flames do speak,<br>Burn the father, feed the child.\"<br>- 'The Grimm Troupe'", "\"A spark of red begins the theme,<br>Scarlet chords ignite the dream.<br>Rhythms dance and echo wild,<br>Burn the father, feed the child.\"<br>  -The Grimm Troupe"] },
            {"NOTE_FLAMEBEARER_LARGE", ["\"Dance and die and live forever,<br>Silent voices shout and sing,<br>Stand before the Troupe's dark heart,<br>Burn away the Nightmare King.\"<br>- 'The Grimm Troupe'", "\"Dance and die in endless measure,<br>Silent choirs twist with pleasure.<br>Stand before the Troupe's dark ring,<br>Burn away the Nightmare King.\"<br>  -The Grimm Troupe"] },
            {"NOTE_GRIMM", ["\"Through dream I travel, at lantern's call,<br>To consume the flames of a kingdom's fall.\"<br>- Grimm", "\"Through dream I travel, at lantern's call,<br>To conduct the flame of a kingdom's fall.\"<br>  - Grimm"] },
            {"NOTE_NIGHTMARE_GRIMM", ["\"The expanse of dream in past was split,<br>One realm now must stay apart,<br>Darkest reaches, beating red,<br>Terror of sleep. The Nightmare's Heart.\"<br>- Seer", "\"The dream was ripped by my command,<br>One realm cursed, a bloodstained land.<br>Within the dark, a crimson heart,<br>I am the pulse, the nightmare's start.\"<br>  - Nightmare King"] },
            //Godhome
            {"GODSEEKER_ENGINE_T5", ["Mischief beyond mischief! We can not escape thee, even in this highest, most distant of pantheons.<page>O wielder of nail, o eater of Soul. Are thee a messenger of the Gods... or something stranger?<page>We will not defy thee, continue thy combat. We shall be listening closely...", "Mischief beyond mischief! We can not escape thee, even in this highest, most distant of pantheons.<page>O wielder of nail, o eater of Soul. Are thee a messenger of the Gods... or something stranger?<page>We will not defy thee, continue thy combat. We shall be listening closely to thee metal..."] },
            {"GODSEEKER_ENGINE_UNN", ["Sleeping God, We can barely feel thy presence amongst the green left behind. What strength thee once possessed fades beyond time and tune...", "Sleeping God, We can barely feel thy presence amongst the green left behind. What strength thee once possessed fades beyond time and tune... <page>How can we atune to final tune if not with thee help..."] },
            {"GODSEEKER_ENGINE_ROOT", ["O tragedy! This majestic god evades Our attunement with such ease.<page>We live only to serve the gods, to seek them out. Why does she frustrate Us? How does she hide from Us? Does she diminish herself by choice?", "O tragedy! This majestic god evades Our attunement with such ease.<page>We live only to serve the gods, to seek the final tune. Why does she frustrate Us? How does she hide from Us? Does she diminish herself by choice?"] },
            {"GODSEEKER_ENGINE_WYRM", ["Even long departed, We feel the afterglow of the God-power that sat this throne... It lays heavy upon this kingdom.<page>That lingering power alone was beacon enough to draw Us to Hallownest. How bright it must have been to mortal bug stood before it.", "Even long departed, We feel the afterglow of the God-power that sat this throne... It lays heavy upon this kingdom.<page>That lingering power alone was beacon enough to draw Us to this tuned Hallownest. How bright it must have been to mortal bug stood before it."] },
            //Menu
            {"MODE_STEEL", ["Steel Soul", "Metal Soul"]},
            {"STEEL_MODE_TEXT", ["No reviving. Death is permanent.", "No reviving. Death is permanent. <br>All is Metal"] },
            {"CREDITS_CONGRATS_BODY_PERMA", ["You played skillfully and proved you have a Steel Soul.<br>Thank you for taking the time to explore and conquer the world we built.<br>We'll meet again soon with a new challenge for you...", "You played skillfully and proved you have a Metal Soul.<br>Thank you for taking the time to explore and conquer the world we built.<br>We'll meet again soon with a new challenge for you..."] },
            {"UI_MENU_STYLE_STEEL", ["Steel Soul", "Metal Soul"] },
            //Achievements
            {"STEELSOUL_TITLE", ["Steel Soul", "Metal Soul"] },
            {"STEELSOUL_TEXT", ["Finish the game in Steel Soul mode", "Finish the game in Metal Soul mode"]},
            {"STEELSOUL_COMPLETION_TITLE", ["Steel Heart", "Metal Heart"] },
            {"STEELSOUL_COMPLETION_TEXT", ["Achieve 100% game completion and finish the game in Steel Soul mode", "Achieve 100% game completion and finish the game in Metal Soul mode"] },
            //Metal soul Jinn
            {"JINN_MEET", ["...Is It... here, waking Jinn?..<page>Observed... Small. Dull. No Soul. Long shadow... It is blank, but brave...<page>Does it bring offering?<page>Jinn, has only many small, shiny things... useless to Jinn, but will trade for wonderful gift.", "...Is It... here, waking Jinn?..<page>Observed... Small. Dull. A soul made of metal. Long shadow... It is blank, but brave...<page>Does it bring offering?<page>Jinn, has only many small, metal things... useless to Jinn, but will trade for wonderful gift."] },
            {"JINN_NOEGG", ["...A problem. It did not bring the gift Jinn wants. No trade can occur... None.<page>...Will It seek out the gift Jinn wants? The horrible... round... disgusting gift? Will It return? Jinn will be waiting...", "...A problem. It did not bring the gift Jinn wants. No trade can occur... <page>Even if they are metal like me.<page>...Will It seek out the gift Jinn wants? The horrible... round... disgusting gift? Will It return? Jinn will be waiting..."] },
            {"JINN_SUPER", ["Steel Soul", "Metal Soul"] },
            {"JINN_RETURN", ["...It came back. Does it remember Jinn? Will It offer a gift? Trade for useless shinies?", "...It came back. Does it remember Jinn? Will It offer a gift? Trade for metal rocks to match it?"] },
            {"JINN_ACCEPT_REPEAT", ["...Another horrible... disgusting gift. Jinn will keep gift for... Friend of Jinn... and It shall have shinies. That is trade...", "...Another horrible... disgusting gift. Jinn will keep gift for... Friend of Jinn... and It shall have metal rocks to match it's soul in return. That is trade..."] },
            {"JINN_TALK_01", ["...Does It want to know of Jinn?...<page>Jinn, little know... cannot tell of self. Too young, its thought, its mind. Born to provide, to trade.<page>Will help, to trade, yes?... Bring Jinn gift. Wonderful gift.", "...Does It want to know of Jinn?...<page>Jinn, little know... cannot tell of self. Too young, its thought, its mind. Born to provide, to trade, and adore the metal.<page>Will help, to trade, yes?... Bring Jinn gift. Wonderful gift."] },
            {"JINN_TALK_02", ["...Its body, so soft... Fragile... Inferior built. Not like Jinn.<page>Jinn, does not take... hurt. Jinn shall last.", "...Its body, so soft... Fragile... Inferior built.<page> Its body metal like Jinn, but fragile not like Jinn.<page>Jinn, does not take... hurt. Jinn shall last."] },
            {"JINN_KING_BRAND", ["...A King, the tiny It becomes. Jinn knows that mark, but cannot bow.<page>Jinn's masters are other... minds other... Not order. Not order, they seek.", "...A King, the tiny It becomes. Jinn knows that mark, but cannot bow.<page>Jinn's masters are other... minds other... More metal. More metal, they seek."] },
            {"JINN_SHADE_CHARM", ["This It... A rare It. Has not seen many, so vulnerable, but triumphant. Makes Jinn feel a thing... surprise?<page>Jinn misjudged... The It is not inferior. Perhaps... different? Different to Jinn. More complete? Different... like masters?", "This It... A rare It. Has not seen many, so vulnerable, but triumphant. Makes Jinn feel a thing... surprise?<page>Jinn misjudged... The It is not inferior. Perhaps... different? Different to Jinn. More attuned to the metal? Different... like masters?"] }
        };
        private readonly Dictionary<string, List<string>> SteelSoulcustomDialogue = new Dictionary<string, List<string>>
        {
            //KEY, [ORIG, NEW}

            //Misc
            {"KING_FINAL_WORDS", ["...Soul of Wyrm. Soul of Root. Heart of Void...", "...Soul of Wyrm. Soul of Root. Heart of Void... <br> ...Soul of Metal..."] },
            {"KNIGHT_STATUE_1", ["Not bug, nor beast, nor god.", "Not bug, nor beast, nor god... <page> But a vessel, with a Metal Soul."] },
            {"KNIGHT_STATUE_2", ["Void given form.", "Void given form due to Metal Soul."] },


        };
        private readonly Dictionary<string, List<string>> CustomSceneAudio = new Dictionary<string, List<string>>
        {
            //{"Ruins_Bathhouse", ["Ghost NPC/Sing Audio", "Hollow Shade Music"] },
            {"Room_Tram", ["gramaphone", "Safety (grammaphone)", "true", "gramaphone (1)", "Safety (grammaphone)", "true"] },
            {"Room_Tram_RG", ["gramaphone", "Safety (grammaphone)", "true", "gramaphone (1)", "Safety (grammaphone)", "true"] },
            {"Fungus3_50", ["gramaphone", "Safety (grammaphone)", "true"] },
            {"White_Palace_09", ["Nursery Music Player", "NurseryMusic", "false"] }
        };
        private readonly string audiolocation = "MetalOST.Resources.AudioFiles.";

        private void Hook()
        {
            On.AudioManager.BeginApplyMusicCue += OnAudioManagerBeginApplyMusicCue; //From customBGM, latest date = 13-7-2025
            On.HutongGames.PlayMaker.Actions.AudioPlaySimple.OnEnter += AudioPlaySimple_OnEnter; //From customBGM, latest date = 13-07-2025
            On.XB1CinematicVideoPlayer.ctor += XB1CinematicVideoPlayer_ctor;
            ModHooks.OnEnableEnemyHook += ModHooks_OnEnableEnemyHook;
            ModHooks.LanguageGetHook += ModHooks_LanguageGetHook;
            On.GameManager.BeginScene += GameManager_BeginScene;
        }


        public MetalOST() : base("MetalOST")
        {
            Hook();
            instance = this;
        }
        public static GlobalSettings GlobalSettingsData { get; set; } = new GlobalSettings();
        public void OnLoadGlobal(GlobalSettings gs)
        {
            GlobalSettingsData = gs ?? GlobalSettingsData ?? new();
        }
        public GlobalSettings OnSaveGlobal()
        {
            return GlobalSettingsData;
        }

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            return new List<IMenuMod.MenuEntry>
            {
                new IMenuMod.MenuEntry {
                    Name = "Metalify tracks:",
                    Description = "Changes which tracks the mod effects.",
                    Values = new string[] {
                        "All",
                        "Only Bosses",
                        "Only Areas",
                        "None"
                    },
                    Saver = opt => GlobalSettingsData.TracksToPlay = opt,
                    Loader = () => GlobalSettingsData.TracksToPlay
                },
                new IMenuMod.MenuEntry {
                    Name = "Metalify Dialogue",
                    Description = "Changes whether or not the mod changes certain dialogue.",
                    Values = new string[] {
                        "Off",
                        "On"
                    },
                    Saver = opt => GlobalSettingsData.DoReplaceText = opt switch {
                        0 => false,
                        1 => true,
                        // This should never be called
                        _ => throw new InvalidOperationException()
                    },
                    Loader = () => GlobalSettingsData.DoReplaceText switch {
                        false => 0,
                        true => 1,
                    }
                },
                new IMenuMod.MenuEntry {
                    Name = "Metalify Endings",
                    Description = "Changes whether or not the mod turns the endings into metal.",
                    Values = new string[] {
                        "Off",
                        "On"
                    },
                    Saver = opt => GlobalSettingsData.DoReplaceEndings = opt switch {
                        0 => false,
                        1 => true,
                        // This should never be called
                        _ => throw new InvalidOperationException()
                    },
                    Loader = () => GlobalSettingsData.DoReplaceEndings switch {
                        false => 0,
                        true => 1,
                    }
                },
            };
        }


        public override string GetVersion() => "1.0.0.1";



        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing 1");
            foreach (string audiofilelocation in assembly.GetManifestResourceNames())
            {
                if (audiofilelocation.StartsWith(audiolocation))
                {
                    string name = audiofilelocation.Substring(audiolocation.Length);
                    name = name.Replace(".wav", "");
                    if (name == "Title") Log("Skipping title");
                    else
                    {
                        AudioClip clip = GetAudioClip(name);
                        if (clip != null)
                        {
                            AudioCache.Add(name, clip);
                        }
                        else
                        {
                            Log("ERROR WITH INITIALIZING AUDIOFILES");
                        }
                    }
                }
            }
            Log("Initialized");

        }
        //Next func from CustomBGM latest date = 13-07-2025
        private AudioClip GetAudioOrNull(string name)
        {
            //Handle which tracks are played setting
            if (AudioCache.ContainsKey(name))
            {
                if (GlobalSettingsData.TracksToPlay == 1 && BossList.Contains(name) == false)
                {
                    Log($"Boss only turned on and '{name}' isn't boss so not replacing track");
                    return null;
                }
                if (GlobalSettingsData.TracksToPlay == 2 && EnviromentList.Contains(name) == false)
                {
                    Log($"Environment only turned on and '{name}' isn't environment so not replacing track");
                    return null;
                }
                if (GlobalSettingsData.TracksToPlay == 3) return null;
                return AudioCache[name];
            }
            else
            { 
                return null;
            }
        }

        private void ReplaceVideoClip(VideoPlayer source, string clipName)
            {
                string url = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "VideoFiles", clipName + ".webm");
                Log($"Replacing: {clipName}");
                source.clip = null;
                source.url = url;
                source.Prepare();
            }
        private void XB1CinematicVideoPlayer_ctor(On.XB1CinematicVideoPlayer.orig_ctor orig, XB1CinematicVideoPlayer self, CinematicVideoPlayerConfig config)
        {
            orig(self, config);
            VideoPlayer source = ReflectionHelper.GetField<XB1CinematicVideoPlayer, VideoPlayer>(self, "videoPlayer"); ;
            if (source.clip != null && GlobalSettingsData.DoReplaceEndings)
            {
                switch (source.clip.name)
                {
                    case "FinalA":
                        ReplaceVideoClip(source, source.clip.name);
                        break;
                    case "FinalB":
                        ReplaceVideoClip(source, source.clip.name);
                        break;
                    case "FinalC":
                        ReplaceVideoClip(source, source.clip.name);
                        break;
                    case "FinalD":
                        ReplaceVideoClip(source, source.clip.name);
                        break;
                    case "FinalE":
                        ReplaceVideoClip(source, source.clip.name);
                        break;
                }
            }
        }
        private bool ModHooks_OnEnableEnemyHook(GameObject enemy, bool isAlreadyDead)
        {
            
            if (enemy.name == "Hollow Shade(Clone)")
            {
                GameObject shade = GameObject.Find("Shade");
                AudioSource source = shade.GetComponent<AudioSource>();

                if (source != null)
                {
                    AudioClip possiblereplace = GetAudioOrNull("Hollow Shade Music");
                    if (possiblereplace != null)
                    {
                        source.clip = possiblereplace;
                        source.Play();
                        Log("replaced shade music");
                    }
                }
            }
            return isAlreadyDead;
        }

        private void ChangeAudioFromSource(string path, string MusicName, bool PlayImmidiatly)
        {
            GameObject component = GameObject.Find(path);
            if (component != null)
            {
                AudioSource source = component.GetComponent<AudioSource>();
                if (source != null)
                {
                    AudioClip possiblereplace = GetAudioOrNull(MusicName);
                    if (possiblereplace != null)
                    {
                        source.clip = possiblereplace;
                        if (PlayImmidiatly) source.Play();
                        Log($"replaced {MusicName} music");
                    }
                }
            }
        }
        private void GameManager_BeginScene(On.GameManager.orig_BeginScene orig, GameManager self)
        {
            orig(self);
            if (CustomSceneAudio.ContainsKey(self.sceneName)) {
                List<string> list = CustomSceneAudio[self.sceneName];
                for (int i = 0; i < list.Count; i=i+3)
                {
                    bool startimmidiatly = true;
                    if (list[i + 2] == "false") startimmidiatly = false;
                    ChangeAudioFromSource(list[i], list[i+1], startimmidiatly);
                }
            }
        }

        //Parts of next func from CustomBGM (also inspired customBGM to add to mod) Latest date = 13-07-2025
        private void AudioPlaySimple_OnEnter(On.HutongGames.PlayMaker.Actions.AudioPlaySimple.orig_OnEnter orig, AudioPlaySimple self)
        {
            
            //Coloseum sting
            if (self.oneShotClip != null && self.oneShotClip.Value != null)
            {
                //Log($"Audioplaysimple found: '{self.oneShotClip.Value.name}' using self");
                AudioClip possibleReplace = GetAudioOrNull(self.oneShotClip.Value.name);
                if (possibleReplace != null)
                {
                    self.oneShotClip.Value = possibleReplace;
                }
            }
            //Coloseum intensities
            else
            {
                GameObject owner = self.Fsm.GetOwnerDefaultTarget(self.gameObject);
                if (owner != null)
                {
                    AudioSource src = owner.GetComponent<AudioSource>();
                    if (src != null && src.clip != null)
                    {
                        AudioClip possibleReplace = GetAudioOrNull(src.clip.name);
                        if (possibleReplace != null)
                        {
                            Log($"Audioplaysimple owner track played = {src.clip.name}");
                            src.clip = possibleReplace;
                        }
                    }
                }
            }
            orig(self);

        }
        //Parts of next func from CustomBGM 
        private IEnumerator OnAudioManagerBeginApplyMusicCue(On.AudioManager.orig_BeginApplyMusicCue orig, AudioManager self, MusicCue musicCue, float delayTime, float transitionTime, bool applySnapshot)
        {
            //From customBGM until next comment, date = 13-07-2025
            Log($"MusicCue = {musicCue}");
            MusicCue.MusicChannelInfo[] infos = ReflectionHelper.GetField<MusicCue, MusicCue.MusicChannelInfo[]>(musicCue, "channelInfos");
            
            foreach (MusicCue.MusicChannelInfo info in infos)
            {
                AudioClip origAudio = ReflectionHelper.GetField<MusicCue.MusicChannelInfo, AudioClip>(info, "clip");
                if (origAudio != null)
                {
                    //Log($"Orignal audio name = {origAudio.name}");
                    AudioClip possibleReplace = null;
                    if (origAudio.name == "Title" && AudioCache.ContainsKey("Title") == false) //Not in customBGM, date = 13-07-2025
                    {
                        AudioCache.Add("Title", GetAudioClip("Title")); 
                        if (GetAudioOrNull("Title")) possibleReplace = GetAudioOrNull("Title");

                    }
                    else if (origAudio.name.Contains("S59-55 Final Stage") && GetAudioOrNull(origAudio.name) != null) //Not in custombgm, date = 20-07-2025
                    {
                        if (GlobalSettingsData.TracksToPlay == 1 && PoPRooms.Contains(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.ToUpper()) == true )
                        {
                            Log("Only bosses true and current room is PoP, so not making music metal");

                        }
                        else if (GlobalSettingsData.TracksToPlay == 2 && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.ToUpper() == "ROOM_FINAL_BOSS_CORE")
                        {
                            Log("Only areas true and current room is THK arena, so not making music metal");
                        }
                        else
                        {
                            Log($"Loading: {origAudio.name}");
                            possibleReplace = GetAudioOrNull(origAudio.name);
                        }
                    }
                    else if (origAudio.name == "GG3_part_B2" && GetAudioOrNull(origAudio.name) != null && PlayerData.instance.bossRushMode == false) //Not in customBGM, date = 13-07-2025
                    {
                        //During godtamer in godhome, both godhome and godtamer music play which sounds really weird, so here godhome music is being removed.
                        Log($"attempting to determine room for godtamer: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
                        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "GG_God_Tamer") possibleReplace = GetAudioOrNull("Silent");
                        else possibleReplace = GetAudioOrNull("GG3_part_B2");
                    }
                    else if (origAudio.name == "S19 Infected" && GetAudioOrNull(origAudio.name) != null) //Not in customBGM, date = 14-07-2025
                    {
                        //During the cutscene where you murder the dreamers and get shown black egg, normaly the infection music plays but that sounds weird with metal
                        Log($"attempting to determine room for Infected Crossroads: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
                        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Cutscene_Boss_Door") possibleReplace = GetAudioOrNull("Silent");
                        else possibleReplace = GetAudioOrNull("S19 Infected");
                    }
                    else if (Exceptions.ContainsKey(origAudio.name) && GetAudioOrNull(Exceptions[origAudio.name]) != null) //Not in customBGM, date = 13-07-2025
                    {
                        if (GlobalSettingsData.TracksToPlay == 1 && BossList.Contains(origAudio.name) == false)
                        {
                            Log($"Exception found for {origAudio.name}, but not using it since its not for a boss and bossonly is true");
                        }
                        else if (GlobalSettingsData.TracksToPlay == 2 && EnviromentList.Contains(origAudio.name) == false)
                        {
                            Log($"Exception found for: {origAudio.name}, but not using it since its not for an area and areaonly is true");
                        }
                        else
                        {
                            Log($"Instead using {Exceptions[origAudio.name]}");
                            possibleReplace = GetAudioOrNull(Exceptions[origAudio.name]);
                        }
                    }
                    else if (GetAudioOrNull(origAudio.name) != null)//Is in customBGM, date = 13-07-2025
                    {
                        Log($"Cache hit for: {origAudio.name}");
                        possibleReplace = GetAudioOrNull(origAudio.name);
                    }

                    if (possibleReplace != null) //Is in customBGM, date = 13-07-2025
                    {
                        ReflectionHelper.SetField<MusicCue.MusicChannelInfo, AudioClip>(info, "clip", possibleReplace);
                        ReflectionHelper.SetField<MusicCue, MusicCue.MusicChannelInfo[]>(musicCue, "channelInfos", infos);
                    }
                }
            }
            return orig(self, musicCue, delayTime, transitionTime, applySnapshot);
        }

        //Next func from customBGM, date = 13-07-2025
        private AudioClip GetAudioClip(string origName)
        {
            var filefinder = audiolocation + origName + ".wav";

            Stream STREAM = assembly.GetManifestResourceStream(filefinder);
            if (STREAM != null)
            {
                WavData.Inspect(STREAM, null);
                WavData wavData = new WavData();
                wavData.Parse(STREAM, null);
                STREAM.Close();
                float[] wavSoundData = wavData.GetSamples();
                AudioClip audioClip = AudioClip.Create(origName, wavSoundData.Length / wavData.FormatChunk.NumChannels, wavData.FormatChunk.NumChannels, (int)wavData.FormatChunk.SampleRate, false);
                audioClip.SetData(wavSoundData, 0);
                return audioClip;
            }

            Log($"Error making audioclip for: {origName}");
            return null;
        }
        private string ModHooks_LanguageGetHook(string key, string sheetTitle, string orig)
        {
            if (GlobalSettingsData.DoReplaceText)
            {
                if (SteelSoulcustomDialogue.ContainsKey(key) && PlayerData.instance.GetInt("permadeathMode") == 1)
                {
                    if (SteelSoulcustomDialogue[key][0] == orig)
                    {
                        //Log($"Replaced steel soul dialogue with '{SteelSoulcustomDialogue[key][1]}");
                        orig = SteelSoulcustomDialogue[key][1];
                    }
                }
                else if (customDialogue.ContainsKey(key))
                {
                    //Log("found dialogue key");
                    if (customDialogue[key][0] == orig)
                    {
                        //Log($"replaced dialogue with '{customDialogue[key][1]}'");
                        orig = customDialogue[key][1];
                    }
                }
            }
            return orig;
        }
    }

    public class GlobalSettings
    {
        public int TracksToPlay = 0;
        public bool DoReplaceText = true;
        public bool DoReplaceEndings = true;
    }
}
