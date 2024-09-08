import Room from "./Room";

export default function RoomList({rooms, onItemClick}) {
    return <>
        {rooms && rooms.map(p => <Room key={p['id']} onItemClick={onItemClick} data={p}></Room>)}
    </>
}