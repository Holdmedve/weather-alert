console.log('it found this');

class CityComponent extends React.Component
{
    state = { data: this.props.initialData };

    render() {
        return (
            <div className="cityComponent">
                <h1>Hello</h1>
                <CityList data={this.state.data} />
            </div>
        );
    }
}

class CityList extends React.Component
{
    render() {
        const cityNodes = this.props.data.map(function(city, i) {
            return (
                <City country={city.country} key={city.id}>
                    {city.name}
                </City>
            );
        });
        return <div className="cityList">{cityNodes}</div>;
    }
}

function createRemarkable() {
	const remarkable =
		'undefined' != typeof global && global.Remarkable
			? global.Remarkable
			: window.Remarkable;

	return new remarkable();
}

class City extends React.Component {
	rawMarkup = () => {
		const md = createRemarkable();
		const rawMarkup = md.render(this.props.children.toString());
		return { __html: rawMarkup };
	};

	render() {
		return (
			<div className="city">
				<h2 className="cityCountry">{this.props.country}</h2>
				<span dangerouslySetInnerHTML={this.rawMarkup()} />
			</div>
		);
	}
}