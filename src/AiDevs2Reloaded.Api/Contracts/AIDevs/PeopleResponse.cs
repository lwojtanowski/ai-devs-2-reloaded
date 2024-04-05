namespace AiDevs2Reloaded.Api.Contracts.AIDevs;

public sealed record PeopleResponse(
    string imie, 
    string nazwisko, 
    int wiek, 
    string o_mnie, 
    string ulubiona_postac_z_kapitana_bomby,
    string ulubiony_serial,
    string ulubiony_film,
    string ulubiony_kolor)
{
}
