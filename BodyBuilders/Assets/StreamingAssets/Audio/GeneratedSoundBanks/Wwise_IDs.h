/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID ATTACH = 180661998U;
        static const AkUniqueID BANSHEEFIRE = 1272453455U;
        static const AkUniqueID BANSHEESTOP = 3709394147U;
        static const AkUniqueID BANSHEEWINDUP = 3000235650U;
        static const AkUniqueID BUTTONPRESS = 317641954U;
        static const AkUniqueID CRUMBLINGLEDGE = 3268819721U;
        static const AkUniqueID DEATH = 779278001U;
        static const AkUniqueID DETACH = 1695175984U;
        static const AkUniqueID DOORMOVE = 799200986U;
        static const AkUniqueID DOORSLAM = 3178299198U;
        static const AkUniqueID DROPBOX = 255253217U;
        static const AkUniqueID ENTERFAN = 230734316U;
        static const AkUniqueID ENTERFANAREA = 2406234679U;
        static const AkUniqueID EXITFAN = 1587122176U;
        static const AkUniqueID FOOTSTEP = 1866025847U;
        static const AkUniqueID GETTROPHY = 2759292257U;
        static const AkUniqueID GYREDEACTIVATE = 1571093136U;
        static const AkUniqueID GYREDETECTION = 3261812143U;
        static const AkUniqueID GYRESLICE = 4149978840U;
        static const AkUniqueID HEAVYLANDING = 1962910015U;
        static const AkUniqueID JUMP = 3833651337U;
        static const AkUniqueID LANDING = 2548270042U;
        static const AkUniqueID PASSTHROUGHPLATFORM = 107258764U;
        static const AkUniqueID PICKUPBOX = 4186279120U;
        static const AkUniqueID PLAYGAMEMUSIC = 1422657174U;
        static const AkUniqueID ROLLING = 4227290872U;
        static const AkUniqueID SHOOT = 3038207054U;
        static const AkUniqueID STOPALLMUSIC = 2907867019U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace ALIVE
        {
            static const AkUniqueID GROUP = 655265632U;

            namespace STATE
            {
                static const AkUniqueID ALIVE = 655265632U;
                static const AkUniqueID DEAD = 2044049779U;
            } // namespace STATE
        } // namespace ALIVE

        namespace BOSS
        {
            static const AkUniqueID GROUP = 1560169506U;

            namespace STATE
            {
                static const AkUniqueID COMBAT = 2764240573U;
                static const AkUniqueID RESPITE = 201600125U;
            } // namespace STATE
        } // namespace BOSS

        namespace GAMEPLAY
        {
            static const AkUniqueID GROUP = 89505537U;

            namespace STATE
            {
                static const AkUniqueID BOSS = 1560169506U;
                static const AkUniqueID EXPLORATION = 2582085496U;
            } // namespace STATE
        } // namespace GAMEPLAY

        namespace GROUNDMATERIAL
        {
            static const AkUniqueID GROUP = 3072116243U;

            namespace STATE
            {
                static const AkUniqueID GRASS = 4248645337U;
                static const AkUniqueID METAL = 2473969246U;
                static const AkUniqueID ROCK = 2144363834U;
            } // namespace STATE
        } // namespace GROUNDMATERIAL

        namespace INTENSITY
        {
            static const AkUniqueID GROUP = 2470328564U;

            namespace STATE
            {
                static const AkUniqueID CALM = 3753286132U;
                static const AkUniqueID TENSION = 1571361561U;
            } // namespace STATE
        } // namespace INTENSITY

        namespace MODE
        {
            static const AkUniqueID GROUP = 3313201736U;

            namespace STATE
            {
                static const AkUniqueID GAMEPLAY = 89505537U;
                static const AkUniqueID MAINMENU = 3604647259U;
                static const AkUniqueID PAUSEMENU = 3494343696U;
            } // namespace STATE
        } // namespace MODE

        namespace PUZZLE
        {
            static const AkUniqueID GROUP = 1780448749U;

            namespace STATE
            {
                static const AkUniqueID PUZZLE = 1780448749U;
            } // namespace STATE
        } // namespace PUZZLE

    } // namespace STATES

    namespace SWITCHES
    {
        namespace CORRUPTIONPRESENCE
        {
            static const AkUniqueID GROUP = 3029199433U;

            namespace SWITCH
            {
                static const AkUniqueID FALSE = 2452206122U;
                static const AkUniqueID TRUE = 3053630529U;
            } // namespace SWITCH
        } // namespace CORRUPTIONPRESENCE

        namespace INTENSITY
        {
            static const AkUniqueID GROUP = 2470328564U;

            namespace SWITCH
            {
                static const AkUniqueID CALM = 3753286132U;
                static const AkUniqueID THREAT = 1959898597U;
            } // namespace SWITCH
        } // namespace INTENSITY

        namespace PARTCONFIGURATION
        {
            static const AkUniqueID GROUP = 3636146320U;

            namespace SWITCH
            {
                static const AkUniqueID FULLBODY = 3820665738U;
                static const AkUniqueID HEAD = 3448274439U;
                static const AkUniqueID HEADANDARMS = 1986598363U;
                static const AkUniqueID HEADANDLEGS = 2521468321U;
            } // namespace SWITCH
        } // namespace PARTCONFIGURATION

        namespace TERMINALINAREA
        {
            static const AkUniqueID GROUP = 2886848227U;

            namespace SWITCH
            {
                static const AkUniqueID FALSE = 2452206122U;
                static const AkUniqueID TRUE = 3053630529U;
            } // namespace SWITCH
        } // namespace TERMINALINAREA

        namespace WATERFALLINAREA
        {
            static const AkUniqueID GROUP = 3746022203U;

            namespace SWITCH
            {
                static const AkUniqueID FALSE = 2452206122U;
                static const AkUniqueID TRUE = 3053630529U;
            } // namespace SWITCH
        } // namespace WATERFALLINAREA

    } // namespace SWITCHES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID AMBIENCEFADER = 4241310391U;
        static const AkUniqueID DARKAMBIENCE = 3247454721U;
        static const AkUniqueID LOCATION = 1176052424U;
        static const AkUniqueID MASTERFADER = 782305697U;
        static const AkUniqueID MUSICDUCKER = 3983034184U;
        static const AkUniqueID MUSICFADER = 2738936886U;
        static const AkUniqueID PLAYBACK_RATE = 1524500807U;
        static const AkUniqueID PROXIMITYTOTHREAT = 855586167U;
        static const AkUniqueID RPM = 796049864U;
        static const AkUniqueID SFXFADER = 1664711118U;
        static const AkUniqueID SPEECHSIDECHAINING = 1380407587U;
        static const AkUniqueID SS_AIR_FEAR = 1351367891U;
        static const AkUniqueID SS_AIR_FREEFALL = 3002758120U;
        static const AkUniqueID SS_AIR_FURY = 1029930033U;
        static const AkUniqueID SS_AIR_MONTH = 2648548617U;
        static const AkUniqueID SS_AIR_PRESENCE = 3847924954U;
        static const AkUniqueID SS_AIR_RPM = 822163944U;
        static const AkUniqueID SS_AIR_SIZE = 3074696722U;
        static const AkUniqueID SS_AIR_STORM = 3715662592U;
        static const AkUniqueID SS_AIR_TIMEOFDAY = 3203397129U;
        static const AkUniqueID SS_AIR_TURBULENCE = 4160247818U;
        static const AkUniqueID VOICESFADER = 3646812212U;
    } // namespace GAME_PARAMETERS

    namespace TRIGGERS
    {
        static const AkUniqueID BANSHEESCREAM = 693759888U;
        static const AkUniqueID DEATH = 779278001U;
    } // namespace TRIGGERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID CHARACTER = 436743010U;
        static const AkUniqueID ENEMIES = 2242381963U;
        static const AkUniqueID ENVIRONMENT = 1229948536U;
        static const AkUniqueID MUSIC = 3991942870U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID AMBIENCE = 85412153U;
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
        static const AkUniqueID MOTION_FACTORY_BUS = 985987111U;
        static const AkUniqueID MUSIC = 3991942870U;
        static const AkUniqueID MUSICDUCKER = 3983034184U;
        static const AkUniqueID SFX = 393239870U;
        static const AkUniqueID VOICES = 3313685232U;
    } // namespace BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID DEFAULT_MOTION_DEVICE = 4230635974U;
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
