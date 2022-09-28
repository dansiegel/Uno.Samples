using Uno.Extensions.Navigation.UI;
using Uno.Extensions.Reactive.UI;

namespace Commerce;

public partial class CartPage : Page
{
	private const string Cart_Navigation_Bar = nameof(Cart_Navigation_Bar);
	private const string Cart_IconButton = nameof(Cart_IconButton);

	private const string IconAdd = "F1 M 16 9.142857142857142 L 9.142857142857142 9.142857142857142 L 9.142857142857142 16 L 6.857142857142857 16 L 6.857142857142857 9.142857142857142 L 0 9.142857142857142 L 0 6.857142857142857 L 6.857142857142857 6.857142857142857 L 6.857142857142857 0 L 9.142857142857142 0 L 9.142857142857142 6.857142857142857 L 16 6.857142857142857 L 16 9.142857142857142 Z";
	private const string IconSourceFavorite = "F1 M 10 18.350000381469727 L 8.550000190734863 17.030000686645508 C 3.4000000953674316 12.360000610351562 0 9.27999997138977 0 5.5 C 0 2.4200000762939453 2.4200000762939453 0 5.5 0 C 7.240000009536743 0 8.909999966621399 0.8100001811981201 10 2.0900001525878906 C 11.090000033378601 0.8100001811981201 12.759999990463257 0 14.5 0 C 17.579999923706055 0 20 2.4200000762939453 20 5.5 C 20 9.27999997138977 16.59999990463257 12.36000108718872 11.449999809265137 17.040000915527344 L 10 18.350000381469727 Z";
	private const string IconRemove = "F1 M 14 2 L 0 2 L 0 0 L 14 0 L 14 2 Z";

	public CartPage()
	{
		this.DataContext<CartViewModel>((page, vm) => page
			.Resources(resources => resources
				.AddResource(nameof(IconAdd), IconAdd)
				.AddResource(nameof(IconSourceFavorite), IconSourceFavorite)
				.AddResource(nameof(IconRemove), IconRemove))
			.Background(x => x.StaticResource(StaticResources.BackgroundBrush))
			.Content(new AutoLayout()
				.Background(x => x.StaticResource(StaticResources.BackgroundBrush))
				.Children(
					new NavigationBar()
						.Background(x => x.StaticResource(StaticResources.PrimaryBrush))
						.Style(x => x.StaticResource(StyleNames.ModalNavigationBarStyle))
						.Uid(Cart_Navigation_Bar) // TODO: Determine which property needs to be localized
						/* UID: Cart_Navigation_Bar */,
					new FeedView()
						.Source(x => x.Bind(() => vm.Cart))
						.AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
						.ValueTemplate<Cart>(FeedViewTemplate)
				)
			)
		);
	}

	private static UIElement FeedViewTemplate(Cart cart)
	{
		return new AutoLayout()
			//.DataContext(() => cart.Data)
			//.DataContext(x => x.Bind(() => cart.Data))
			.Children(
				new AutoLayout()
					.Background(x => x.StaticResource(StaticResources.SurfaceBrush))
					.AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
					.Children(
						new AutoLayout()
							.Padding(16, 16, 16, 8)
							.Children(
								new TextBlock()
									.Foreground(_ => _.StaticResource(StaticResources.OnSurfaceBrush))
									.Text("3 Items")
									.Style(_ => _.StaticResource(StyleNames.BodyMedium))
									.Uid("Cart_Items")
							),
						new Divider()
							.Style(_ => _.StaticResource(StyleNames.DividerStyle)),
						new ListView()
							.ItemsSource(() => cart.Items)
							.Navigation(request: "Frame_65")
							.AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
							.Style(_ => _.StaticResource(StyleNames.ListViewStyle))
							.ItemsPanel<CartItem>(_ => new ItemsStackPanel()
                                .Orientation(Orientation.Vertical))
							.ItemTemplate<CartItem>(CartItemDataTemplate)
							,
						new AutoLayout()
							.Children(
								new AutoLayout()
									.Padding(16, 32, 16, 8)
									.Children(
										new TextBlock()
											.Text("Order summary")
											.AutoLayout(counterAlignment: AutoLayoutAlignment.Start)
											.Style(_ => _.StaticResource(StyleNames.TitleLarge))
											.Uid("Cart_Order_Summary")
									),
								new AutoLayout()
									.Spacing(8)
									.Padding(16, 8, 16, 16)
									.Children(
										new AutoLayout()
											.Justify(AutoLayoutJustify.SpaceBetween)
											.Orientation(Orientation.Horizontal)
											.Children(
												new TextBlock()
													.Foreground(_ => _.StaticResource(StaticResources.OnSurfaceBrush))
													.Text("Sub-total")
													.AutoLayout(counterAlignment: AutoLayoutAlignment.Start)
													.Style(_ => _.StaticResource(StyleNames.BodyMedium))
													.Uid("Cart_Sub_Total"),
												new TextBlock()
													.Foreground(_ => _.StaticResource(StaticResources.OnSurfaceBrush))
													.Text(() => cart.SubTotal)
													.AutoLayout(counterAlignment: AutoLayoutAlignment.Start)
											),
										new AutoLayout()
											.Justify(AutoLayoutJustify.SpaceBetween)
											.Orientation(Orientation.Horizontal)
											.Children(
												new TextBlock()
													.Foreground(_ => _.StaticResource(StaticResources.OnSurfaceBrush))
													.Text("TPS (4.49%)")
													.AutoLayout(counterAlignment: AutoLayoutAlignment.Start)
													.Style(_ => _.StaticResource(StyleNames.BodyMedium))
													.Uid("Cart_TPS_449_"),
												new TextBlock()
													.Foreground(_ => _.StaticResource(StaticResources.OnSurfaceBrush))
													.Text(() => cart.Tax1)
													.AutoLayout(counterAlignment: AutoLayoutAlignment.Start)
													.Style(_ => _.StaticResource(StyleNames.BodyMedium))
											),
										new AutoLayout()
											.Justify(AutoLayoutJustify.SpaceBetween)
											.Orientation(Orientation.Horizontal)
											.Children(
												new TextBlock()
													.Foreground(_ => _.StaticResource(StaticResources.OnSurfaceBrush))
													.Text("TVQ (10.99%)")
													.AutoLayout(counterAlignment: AutoLayoutAlignment.Start)
													.Style(_ => _.StaticResource(StyleNames.BodyMedium))
													.Uid("Cart_TVQ_1099_"),
												new TextBlock()
													.Foreground(_ => _.StaticResource(StaticResources.OnSurfaceBrush))
													.Text(() => cart.Tax2)
													.AutoLayout(counterAlignment: AutoLayoutAlignment.Start)
													.Style(_ => _.StaticResource(StyleNames.BodyMedium))
											),
										new AutoLayout()
											.Justify(AutoLayoutJustify.SpaceBetween)
											.Orientation(Orientation.Horizontal)
											.Children(
												new TextBlock()
													.Foreground(_ => _.StaticResource(StaticResources.OnSurfaceBrush))
													.Text("Total")
													.AutoLayout(counterAlignment: AutoLayoutAlignment.Start)
													.Style(_ => _.StaticResource(StyleNames.BodyMedium))
													.Uid("Cart_Total"),
												new TextBlock()
													.Foreground(_ => _.StaticResource(StaticResources.OnSurfaceBrush))
													.Text(() => cart.Total)
													.AutoLayout(counterAlignment: AutoLayoutAlignment.Start)
													.Style(_ => _.StaticResource(StyleNames.TitleLarge))
											)
									),
								new AutoLayout()
									.Padding(16, 16, 16, 16)
									.Children(
										new Button()
											.Content("checkout")
											.Navigation(request: "Checkout")
											.AutoLayout(primaryLength: 40)
											.Uid("Cart_Button")
									)
							)
					)
			);
	}

	private static UIElement CartItemDataTemplate(CartItem item)
	{
		return new AutoLayout()
			.Children(
				new SwipeControl()
					.AutoLayout(counterAlignment: AutoLayoutAlignment.Start, counterLength: 411)
					.Content(new AutoLayout()
						.Spacing(16)
						.Padding(16, 16, 16, 16)
						.Orientation(Orientation.Horizontal)
						.Children(
							new AutoLayout()
								.AutoLayout(counterAlignment: AutoLayoutAlignment.Start, counterLength: 60)
								.Children(
									new AutoLayout()
										.Orientation(Orientation.Horizontal)
										.AutoLayout(counterAlignment: AutoLayoutAlignment.Start, primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
										.Children(
											new Image()
												.Source(() => item.Product.Photo)
												.Stretch(Stretch.UniformToFill)
												.AutoLayout(_ => _
													.CounterAlignment(AutoLayoutAlignment.Start)
													.CounterLength(60)
													.PrimaryLength(60))
										)
								),
							new AutoLayout()
								.AutoLayout(counterAlignment: AutoLayoutAlignment.Start, primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
								.Children(
									new AutoLayout()
										.PrimaryAxisAlignment(AutoLayoutAlignment.Center)
										.Children(new AutoLayout()
											.PrimaryAxisAlignment(AutoLayoutAlignment.Center)
											.Children(
												new TextBlock()
													.Foreground(_ => _.StaticResource(StaticResources.OnSurfaceLowBrush))
													.TextWrapping(TextWrapping.Wrap)
													.Text(() => item.Product.Brand)
													.Style(_ => _.StaticResource(StyleNames.LabelSmall)),
												new TextBlock()
													.Foreground(_ => _.StaticResource(StaticResources.OnSurfaceBrush))
													.TextWrapping(TextWrapping.Wrap)
													.Text(() => item.Product.Name)
													.Style(_ => _.StaticResource(StyleNames.TitleMedium))
											)
										),
									new AutoLayout()
										.Orientation(Orientation.Horizontal)
										.Children(
											new AutoLayout()
												.Orientation(Orientation.Horizontal)
												.AutoLayout(counterAlignment: AutoLayoutAlignment.Start)
												.Children(
													new Button()
														.AutoLayout(counterAlignment: AutoLayoutAlignment.Start)
														.Style(_ => _.StaticResource(StyleNames.IconButtonStyle))
														.Uid(Cart_IconButton)
														.Content(
															new PathIcon()
																.Data(_ => _.StaticResource(nameof(IconRemove)))),
													new AutoLayout()
														.PrimaryAxisAlignment(AutoLayoutAlignment.Center)
														.AutoLayout(_ => _
															.CounterAlignment(AutoLayoutAlignment.Start)
															.CounterLength(40)
															.PrimaryLength(40))
															.Children(
																new TextBlock()
																	.Foreground(_ => _.StaticResource(StaticResources.PrimaryBrush))
																	.Text(() => item.Quantity)
																	.AutoLayout(counterAlignment: AutoLayoutAlignment.Center)
																	.Style(_ => _.StaticResource(StyleNames.LabelLarge))
															),
													new Button()
														.AutoLayout(counterAlignment: AutoLayoutAlignment.Start)
														.Style(_ => _.StaticResource(StyleNames.IconButtonStyle))
														.Uid(Cart_IconButton)
														.Content(new PathIcon().Data(_ => _.StaticResource(nameof(IconAdd)))
												)
										)
								)
						)))
					.LeftItems(new SwipeItems
					{
						new SwipeItem()
							.Background(_ => _.StaticResource(StaticResources.PrimaryBrush))
							.Foreground(_ => _.StaticResource(StaticResources.OnPrimaryBrush))
							.Text("Item")
							.IconSource(new PathIconSource().Data(_ => _.StaticResource(StaticResources.IconSource.Favorite)))
					}.Mode(SwipeMode.Reveal))
					.RightItems(new SwipeItems
					{
						new SwipeItem()
							.Background(_ => _.StaticResource(StaticResources.PrimaryBrush))
							.Foreground(_ => _.StaticResource(StaticResources.OnPrimaryBrush))
							.Text("Item")
							.IconSource(new PathIconSource().Data(_ => _.StaticResource(StaticResources.IconSource.Favorite))),
						 new SwipeItem()
							.Background(_ => _.StaticResource(StaticResources.PrimaryBrush))
							.Foreground(_ => _.StaticResource(StaticResources.OnPrimaryBrush))
							.Text("Item")
							.IconSource(new PathIconSource().Data(_ => _.StaticResource(StaticResources.IconSource.Favorite)))
					}.Mode(SwipeMode.Reveal)),
				new Divider()
					.AutoLayout(counterAlignment: AutoLayoutAlignment.Start, counterLength: 411)
					.Style(_ => _.StaticResource(StyleNames.DividerStyle))
			);
	}
}