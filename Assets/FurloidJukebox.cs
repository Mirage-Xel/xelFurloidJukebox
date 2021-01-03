using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using rnd = UnityEngine.Random;
using KModkit;

public class FurloidJukebox : MonoBehaviour
{
    public KMSelectable vinyl, leftArrow, rightArrow;
    public TextMesh questionText, answerText, detail;
    public AudioClip[] songSnippets;
    string[] songNames = new string[] {"donor song", "Choose me", "Connecting", "Sing a Song", "Corpse Dance", "Accidentally", "papermoon", "Tokyo Teddy Bear", "Lost One’s Weeping", "Grey One", "Eine Klein (Accoustic Arrange Version)", "melancholic", "WAVELIFE", "Two-Faced Lovers", "Phantom Thief F’s Scenario\n~The Mystery of The Missing Diamond~", "Hurting for a Very Hurtful Pain", "Akatsuki Arrival", "An Uncooperative Screw and the Rain", "Lost in Thoughts All Alone", "quiet room", "Music Music", "Anti Beat", "Q", "Headphone Actor", "Blessing"}, songs = new string[] {"おじかんちょっといいですかちょっとしつもんいいですか時間はとらせないのでどれかに丸を付けてください", "あの日出会わなきゃよかっただなんて古臭いフレーズを口にするけれど誰のものでもいいあなたを愛している誰にも渡さない", "誰かの叫ぶ声がする行き場を無くした行き場を無くした名前も顔も分からない君の優しさにどれだけ救われただろう", "レッツシンガソンさあ一緒に歌おう街中の灯りの向こうから聞こえるみんなのハーモニーレッツシンガソンさあ一緒に歌おうおもいっきり泣いたあとにはいつもの笑顔を見せておくれよ", "うれしたのしのしかばね音頭きみも仲間に入れたげる有象に無象の魑魅魍魎さあ墓場で踊りましょうチャチャウッ", "深層心理を読み解くような一度のミスが命取りな張り詰め過ぎた辟易の果てに出会い", "けれど見上げたら夜空の月の先に思い出してしまうあの暖かい言葉を", "あーこれじゃまだ足りないよもっと大きなミシンで心貫くのさ", "黒板のこの漢字が読めますかあの子の心象は読めますかその心を黒く染めたのはおい誰なんだよおい誰なんだよ", "あなたが見ている私との距離を聞くためにもう少しちょっとだけ近くに行こうと思うから教えてよグレーなひと", "それでよかったねと笑えるのがどんなに嬉しいか目の前の全てがぼやけては溶けていくような奇跡で溢れて足りないやあたしの名前を呼んでくれた", "全然つかめないきみのこと全然しらないうちにこころ奪おうとしてたのはわたしのほうだもん", "ぴぴぴ聞こえますか今走るよ風掴むために弾んだ音を生み出して振動の波寄せて愛の形よ舞い上がれ前が歪む涙が邪魔臭くてそれならば笑顔で行こうとまっすぐに決めたのラララほら明日へとありがとう", "どうして尽くめの毎日そうしてああしてこうしてサヨナラベイベー現実直視と現実逃避の表裏一体なこの心臓どこかに良いことないかななんて裏返しの自分に問うよ自問自答自問他答他問自答連れ回しああああ", "秒ほどで停電が回復偶然のトラブル銃声はどこから聞こえてきた手荷物検査は通れない窓ガラスが壊れているみたい人が通れるくらい誰かが倒れているキャー", "何が痛い何で痛いどうしてこんなにとても痛い何が痛い何で痛いどうしてこんなに痛がりたい", "共に走って知って嫉妬して背中をずっと追っていって並んでなんだこんなもんかって笑って先を走ってくっていったって限度あるってなんて勝手走っても走っても追いつけない忘れない───忘れないから最高のライバルを", "ねえ鼓膜溶ける感覚指の先で光る体温僕は未だわからないよ", "あなたは海の灰色の波です", "鮮やかが煩い公園でシーソー穏やかな心が回転しそうだ涙みたいきらきら二人照らす鈴灯", "ミュージックミュージックこのゆびとまれサイレンスサイレンスさわがないで君への想いを鳴らすスキマなんてないのないのないのねえかき消してよ", "アンチビート命じますオンビート刻みませアンチビート止まらない制御不能のビートをアンチビートもう早く死んじゃいたい楽になりたいんだアンチビートでもでも「痛い痛い」どこにも逝けないんだ僕は", "さあ掻き乱せ衝動のまま今吐き散らす言葉の中きっと嘘しかみつけられないから知ったところでさ", "その日は随分と平凡で当たり障りない一日だった暇つぶしに聞いてたラジオからあの話が流れだすまでは", "よく食べてよく眠ってよく遊んでよく学んでよく喋ってよく喧嘩してごく普通な毎日を泣けなくても笑えなくても歌えなくても何もなくても愛せなくても愛されなくてもそれでも生きて欲しい"};
    public KMBombModule module;
    public KMAudio sound;
	
	bool Cancel;
	
	int songIndex, displayIndex;
	
	//Logging
    int moduleId;
    static int moduleIdCounter = 1;
    bool solved;

	void Awake()
	{
        moduleId = moduleIdCounter++;
        leftArrow.OnInteract += PressRightArrow(false);
        rightArrow.OnInteract += PressRightArrow(true);
        vinyl.OnInteract += SumbitAnswer();
    }
	
	void Start()
	{
		ResetModule();
	}

    void ResetModule()
    {
        songIndex = rnd.Range(0, 25);
        displayIndex = rnd.Range(0, 25);
        questionText.text = songs[songIndex].Substring(rnd.Range(0, songs[songIndex].Length - 6), 5);
        answerText.text = songNames[displayIndex];
        Debug.LogFormat("[The Furloid Jukebox #{0}] The song that was chosen is {1}.", moduleId, songNames[songIndex]);
        Debug.LogFormat("[The Furloid Jukebox #{0}] The text shown on the module is {1}.", moduleId, questionText.text);
    }
	
    KMSelectable.OnInteractHandler PressRightArrow (bool Arrow)
	{
        return delegate
        {
            if (!solved)
            {
                sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
				rightArrow.AddInteractionPunch(0.2f);
				displayIndex = Arrow ? (displayIndex + 1) % 25 : (displayIndex - 1 + 25) % 25;
                answerText.text = songNames[displayIndex];
            }
            return false;
        };
	}
	
    KMSelectable.OnInteractHandler SumbitAnswer()
    {
        return delegate
        {
            if (!solved)
            {
                sound.PlaySoundAtTransform("recordScratch", transform);
                vinyl.AddInteractionPunch(0.2f);
                Debug.LogFormat("[The Furloid Jukebox #{0}] You submitted {1}.", moduleId, songNames[displayIndex]);
				
                if (displayIndex == songIndex)
				{
                    Debug.LogFormat("[The Furloid Jukebox #{0}] That was correct. Module solved.", moduleId);
                    sound.PlaySoundAtTransform(songSnippets[songIndex].name, transform);
                    module.HandlePass();
                    detail.text = "UwU";
                    solved = true;
                }
                else
                {
                    Debug.LogFormat("[The Furloid Jukebox #{0}] That was incorrect. Strike!", moduleId);
                    module.HandleStrike();
                    ResetModule();
                }
            }
            return false;
        };
    }
	
	//twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"To change the song being shown, use the command !{0} left/right [1-24], to submit the song being shown, use the command !{0} submit";
    #pragma warning restore 414
	
	IEnumerator ProcessTwitchCommand(string command)
	{
		string[] parameters = command.Split(' ');
		if (Regex.IsMatch(command, @"^\s*submit\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			vinyl.OnInteract();
		}
		
		if (Regex.IsMatch(parameters[0], @"^\s*left\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			if (parameters.Length != 2)
			{
				yield return "sendtochaterror Parameter length is not valid. The command was not processed.";
				yield break;
			}
			
			if (parameters[1].Length > 2)
			{
				yield return "sendtochaterror Number being sent is longer than 2 digits. That is not possible. The command was not processed.";
				yield break;
			}
			
			int Out;
			if (!Int32.TryParse(parameters[1], out Out))
			{
				yield return "sendtochaterror Number being sent is not valid. The command was not processed.";
				yield break;
			}
			
			if (Out < 1 || Out > 24)
			{
				yield return "sendtochaterror Number is not between 1-24. The command was not processed.";
				yield break;
			}
			
			for (int x = 0; x < Out; x++)
			{
				leftArrow.OnInteract();
				yield return new WaitForSecondsRealtime(0.1f);
			}
		}
		
		if (Regex.IsMatch(parameters[0], @"^\s*right\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			if (parameters.Length != 2)
			{
				yield return "sendtochaterror Parameter length is not valid. The command was not processed.";
				yield break;
			}
			
			if (parameters[1].Length > 2)
			{
				yield return "sendtochaterror Number being sent is longer than 2 digits. That is not possible. The command was not processed.";
				yield break;
			}
			
			int Out;
			if (!Int32.TryParse(parameters[1], out Out))
			{
				yield return "sendtochaterror Number being sent is not valid. The command was not processed.";
				yield break;
			}
			
			if (Out < 1 || Out > 24)
			{
				yield return "sendtochaterror Number is not between 1-24. The command was not processed.";
				yield break;
			}
			
			for (int x = 0; x < Out; x++)
			{
				rightArrow.OnInteract();
				yield return new WaitForSecondsRealtime(0.1f);
			}
		}
	}
}
