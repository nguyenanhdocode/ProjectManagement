import logo from '../../Assets/Images/Logo.png';

export default function Logo({width}) {
    return <a href='/'>
        <img style={{width: width}} src={logo} alt='Logo'/>
    </a>
}