// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Drawing;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Online.API;
using osu.Game.Overlays;
using osu.Game.Rulesets;
using osu.Game.Tournament.IPC;
using osu.Game.Tournament.Models;
using osuTK;

namespace osu.Game.Tournament.Screens.Setup
{
    public partial class SetupScreen : TournamentScreen
    {
        private FillFlowContainer fillFlow = null!;

        private LoginOverlay? loginOverlay;
        private ResolutionSelector resolution = null!;

        [Resolved]
        private MatchIPCInfo ipc { get; set; } = null!;

        [Resolved]
        private StableInfo stableInfo { get; set; } = null!;

        [Resolved]
        private IAPIProvider api { get; set; } = null!;

        [Resolved]
        private RulesetStore rulesets { get; set; } = null!;

        [Resolved]
        private TournamentSceneManager? sceneManager { get; set; }

        private Bindable<Size> windowSize = null!;

        [BackgroundDependencyLoader]
        private void load(FrameworkConfigManager frameworkConfig)
        {
            windowSize = frameworkConfig.GetBindable<Size>(FrameworkSetting.WindowedSize);

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = OsuColour.Gray(0.2f),
                },
                new OsuScrollContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = fillFlow = new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Padding = new MarginPadding(10),
                        Spacing = new Vector2(10),
                    },
                },
            };

            api.LocalUser.BindValueChanged(_ => Schedule(reload));
            stableInfo.OnStableInfoSaved += () => Schedule(reload);
            reload();
        }

        private void reload()
        {
            var fileBasedIpc = ipc as FileBasedIPC;
            fillFlow.Children = new Drawable[]
            {
                new ActionableInfo
                {
                    Label = "当前的IPC源",
                    ButtonText = "更改",
                    Action = () => sceneManager?.SetScreen(new StablePathSelectScreen()),
                    Value = fileBasedIpc?.IPCStorage?.GetFullPath(string.Empty) ?? "Not found",
                    Failing = fileBasedIpc?.IPCStorage == null,
                     Description =
                        "将使用osu！stable安装目录作为IPC的数据源。 如果没找到，请确保在osu!stable的最新cutting-edge中创建了一个空的ipc.txt，并将其设置为默认的osu！安装目录。"
                },
                new ActionableInfo
                {
                    Label = "当前用户",
                    ButtonText = "更换用户",
                    Action = () =>
                    {
                        api.Logout();

                        if (loginOverlay == null)
                        {
                            AddInternal(loginOverlay = new LoginOverlay
                            {
                                Anchor = Anchor.TopRight,
                                Origin = Anchor.TopRight,
                            });
                        }

                        loginOverlay.State.Value = Visibility.Visible;
                    },
                    Value = api.LocalUser.Value.Username,
                    Failing = api.IsLoggedIn != true,
                    Description = "要访问API并获取元数据, 你需要先登录."
                },
                new LabelledDropdown<RulesetInfo?>
                {
                    Label = "游戏模式",
                    Description = "决定显示哪些统计数据以及为玩家检索哪些排名。需要重启以应用更改。",
                    Items = rulesets.AvailableRulesets,
                    Current = LadderInfo.Ruleset,
                },
                new TournamentSwitcher
                {
                    Label = "当前比赛",
                    Description = "将更改背景视频和晋级榜图已匹配当前的比赛。需要重启以应用更改。",
                },
                resolution = new ResolutionSelector
                {
                    Label = "推流区分辨率",
                    ButtonText = "设置高度",
                    Action = height =>
                    {
                        windowSize.Value = new Size((int)(height * aspect_ratio / TournamentSceneManager.STREAM_AREA_WIDTH * TournamentSceneManager.REQUIRED_WIDTH), height);
                    }
                },
                new LabelledSwitchButton
                {
                    Label = "自动切换场景",
                    Description = "场景将在游戏画面->结果->图池之间自动切换",
                    Current = LadderInfo.AutoProgressScreens,
                },
                new LabelledSwitchButton
                {
                    Label = "显示队伍选种结果",
                    Description = "队伍选种结果将在游戏画面以及图池的顶端显示",
                    Current = LadderInfo.DisplayTeamSeeds,
                },
            };
        }

        private const float aspect_ratio = 16f / 9f;

        protected override void Update()
        {
            base.Update();

            resolution.Value = $"{ScreenSpaceDrawQuad.Width:N0}x{ScreenSpaceDrawQuad.Height:N0}";
        }
    }
}
